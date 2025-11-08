# CockroachDB Migration Handling

This document explains why and how we handle database migrations differently for CockroachDB compared to standard PostgreSQL.

## Table of Contents

- [Why CockroachDB Requires Special Migration Handling](#why-cockroachdb-requires-special-migration-handling)
- [The Locking Problem](#the-locking-problem)
- [SQL Syntax Compatibility Issues](#sql-syntax-compatibility-issues)
- [Implementation Solution](#implementation-solution)
- [Migration Workflow](#migration-workflow)

## Why CockroachDB Requires Special Migration Handling

### Database Compatibility Context

We're using **CockroachDB** as our database, which is:
- PostgreSQL wire protocol compatible
- SQL syntax compatible with PostgreSQL
- **BUT** uses a different concurrency model (MVCC - Multi-Version Concurrency Control)
- **AND** doesn't support some PostgreSQL-specific features

This means we need special handling for:
1. Migration locking mechanisms
2. SQL syntax conversions from SQL Server legacy code

---

## The Locking Problem

### Why EF Core Uses Locks

**Entity Framework Core's migration strategy:**
- Uses PostgreSQL's `LOCK TABLE ... IN ACCESS EXCLUSIVE MODE` to prevent concurrent migrations
- Ensures only one migration runs at a time
- Prevents database schema corruption from concurrent DDL operations

**Standard PostgreSQL flow:**
```sql
LOCK TABLE "__EFMigrationsHistory" IN ACCESS EXCLUSIVE MODE;
-- Then apply migrations
```

### Why CockroachDB Can't Use This

**Problem:**
- CockroachDB **does not support** `LOCK TABLE ... IN ACCESS EXCLUSIVE MODE` syntax
- CockroachDB uses MVCC (Multi-Version Concurrency Control) instead of table-level locks
- When EF Core tries to acquire the lock, CockroachDB throws: `42601: syntax error at or near "lock"`

**Error Example:**
```
fail: Microsoft.EntityFrameworkCore.Database.Command[20102]
      Failed executing DbCommand
      LOCK TABLE "__EFMigrationsHistory" IN ACCESS EXCLUSIVE MODE
Unhandled exception. Npgsql.PostgresException (0x80004005): 42601: at or near "lock": syntax error
```

### Solution

**Our Approach:**
1. Try standard EF Core migration first (in case it works in future CockroachDB versions)
2. Catch the lock error specifically (SQL state `42601` with "lock" in message)
3. Generate the migration SQL script directly
4. Remove all `LOCK TABLE` statements
5. Execute the SQL statements individually
6. Manually record migrations in the history table

**Why This Works:**
- Migrations typically run during deployment (low concurrency)
- CockroachDB's MVCC handles concurrent operations differently
- The migration history table prevents duplicate application
- Errors are logged but don't crash the service

---

## SQL Syntax Compatibility Issues

### Problem 1: SQL Server Identifier Syntax

**Issue:**
Some migration code or generated SQL uses SQL Server identifier syntax with square brackets:
- SQL Server: `[DiscordUserId]`
- PostgreSQL/CockroachDB: `"DiscordUserId"`

**Why This Happens:**
- Legacy code written for SQL Server
- Mixed codebase between SQL Server and PostgreSQL
- EF Core migration builders may generate SQL Server syntax when not properly configured
- Example: `HasFilter("[DiscordUserId] IS NOT NULL")` was using SQL Server brackets

**Error:**
```
42601: at or near "discorduserid": syntax error
```

**Solution:**
Automatically replace SQL Server identifier syntax during SQL execution:
```csharp
.Replace("[", "\"", StringComparison.Ordinal)
.Replace("]", "\"", StringComparison.Ordinal)
```

### Problem 2: SQL Server Date Functions

**Issue:**
The codebase originally had SQL Server date functions:
- SQL Server: `GETUTCDATE()`
- PostgreSQL/CockroachDB: `(now() AT TIME ZONE 'UTC')`

**Why This Happened:**
- `BaseDbContext.cs` originally configured for SQL Server:
  ```csharp
  .HasDefaultValueSql("GETUTCDATE()")
  ```
- This was intended for SQL Server, but we're using CockroachDB

**Error:**
```
42883: unknown function: getutcdate()
```

**Solution:**
1. **Primary Fix:** Updated `BaseDbContext.cs` to use PostgreSQL syntax:
   ```csharp
   .HasDefaultValueSql("(now() AT TIME ZONE 'UTC')")
   ```

2. **Safety Net:** Added SQL replacement during execution:
   ```csharp
   .Replace("GETUTCDATE()", "(now() AT TIME ZONE 'UTC')", StringComparison.OrdinalIgnoreCase)
   ```

---

## Implementation Solution

### Architecture

We created a **shared migration runner** in `Pogo.Shared.Infrastructure` that handles all CockroachDB compatibility:

**Location:** `packages/dotnet-shared/Pogo.Shared.Infrastructure/DatabaseMigrationRunner.cs`

**Usage in each microservice:**
```csharp
app.RunMigrationsAsync<PlayerDbContext>();
```

### How It Works

1. **Asynchronous Execution**
   - Runs migrations in background `Task.Run()`
   - HTTP server starts immediately
   - Migrations don't block application startup

2. **Standard Migration Attempt**
   - First tries `context.Database.MigrateAsync()`
   - Works for standard PostgreSQL or if CockroachDB adds support in future

3. **CockroachDB Lock Error Handling**
   - Catches `PostgresException` with SQL state `42601` and "lock" keyword
   - Falls back to direct SQL execution

4. **SQL Generation & Transformation**
   - Uses `IMigrator.GenerateScript()` to get migration SQL
   - Removes `LOCK TABLE` statements
   - Converts SQL Server syntax to PostgreSQL:
     - `[Identifier]` → `"Identifier"`
     - `GETUTCDATE()` → `(now() AT TIME ZONE 'UTC')`

5. **Direct SQL Execution**
   - Splits SQL by `;` delimiter
   - Executes each statement individually
   - Handles errors gracefully:
     - `42P07`: Table already exists (skip)
     - `42710`: Duplicate object (skip)
     - `42601`: Syntax error (log warning)

6. **Migration History Tracking**
   - Manually inserts migration records into `__EFMigrationsHistory`
   - Uses `ON CONFLICT DO NOTHING` to handle duplicates

### Code Flow

```
app.RunMigrationsAsync<PlayerDbContext>()
  ↓
Task.Run(async => {
  Delay 2 seconds
  ↓
  Try: context.Database.MigrateAsync()
    ↓ (if lock error)
    Catch PostgresException (42601 + "lock")
      ↓
      Get IMigrator service
      ↓
      Generate SQL script
      ↓
      Remove LOCK statements
      ↓
      Replace SQL Server syntax
      ↓
      Execute each SQL statement
      ↓
      Record in migration history
})
```

---

## Migration Workflow

### Normal Flow (Standard PostgreSQL)
1. Service starts
2. `MigrateAsync()` called
3. EF Core acquires lock
4. Migrations applied
5. Service ready

### CockroachDB Flow
1. Service starts
2. `MigrateAsync()` called
3. **Lock acquisition fails** (42601 error)
4. Fallback to direct SQL execution
5. SQL generated and transformed
6. Statements executed individually
7. Migration history recorded manually
8. Service ready

### Error Handling

The implementation is **fault-tolerant**:
- ✅ Service starts even if migrations fail
- ✅ Errors are logged but don't crash the application
- ✅ Migrations retry on next startup
- ✅ Duplicate objects are safely skipped
- ✅ Syntax errors are logged but execution continues

---

## Summary Table

| Issue | SQL Server Syntax | PostgreSQL/CockroachDB Syntax | Why Needed |
|-------|-------------------|-------------------------------|------------|
| **Locks** | `LOCK TABLE ... IN ACCESS EXCLUSIVE MODE` | Not supported | CockroachDB uses MVCC, different concurrency model |
| **Identifiers** | `[ColumnName]` | `"ColumnName"` | Different quoting conventions between database systems |
| **Date Functions** | `GETUTCDATE()` | `(now() AT TIME ZONE 'UTC')` | Different function names and syntax |

---

## Best Practices

1. **Always use PostgreSQL syntax in new code**
   - Use `"ColumnName"` instead of `[ColumnName]`
   - Use `(now() AT TIME ZONE 'UTC')` instead of `GETUTCDATE()`

2. **Test migrations locally first**
   - Run migrations against local CockroachDB instance
   - Verify SQL syntax compatibility

3. **Monitor migration logs**
   - Check for lock errors (expected but should be handled)
   - Watch for syntax errors (may indicate missing conversions)

4. **Migration naming**
   - Use descriptive migration names
   - Include purpose in name (e.g., `UsePostgreSQLNow`)

---

## Related Files

- **Shared Migration Runner:** `packages/dotnet-shared/Pogo.Shared.Infrastructure/DatabaseMigrationRunner.cs`
- **Base DbContext:** `packages/dotnet-shared/Pogo.Shared.Infrastructure/BaseDbContext.cs`
- **Service Usage:** All 5 microservice `Program.cs` files use `app.RunMigrationsAsync<TDbContext>()`

---

## Future Considerations

1. **CockroachDB may add lock support** - Our code tries standard migration first, so it will automatically use locks if CockroachDB adds support
2. **SQL Server removal** - As we remove all SQL Server dependencies, the SQL replacement becomes a safety net
3. **Performance** - Direct SQL execution may be slightly slower but is acceptable for deployment-time operations

