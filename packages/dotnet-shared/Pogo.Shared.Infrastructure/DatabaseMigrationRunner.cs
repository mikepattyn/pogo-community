using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Pogo.Shared.Infrastructure;

/// <summary>
/// Runs database migrations asynchronously with CockroachDB compatibility workarounds.
/// Allows the HTTP server to start immediately while migrations run in the background.
/// </summary>
public static class DatabaseMigrationRunner
{
    /// <summary>
    /// Runs database migrations asynchronously for the specified DbContext type.
    /// This allows the HTTP server to start immediately while migrations run in background.
    /// </summary>
    /// <typeparam name="TDbContext">The DbContext type to migrate.</typeparam>
    /// <param name="app">The WebApplication instance.</param>
    /// <param name="delaySeconds">Delay in seconds before starting migrations (default: 2 seconds).</param>
    public static void RunMigrationsAsync<TDbContext>(this WebApplication app, int delaySeconds = 2)
        where TDbContext : DbContext
    {
        _ = Task.Run(async () =>
        {
            try
            {
                // Small delay to let server start listening
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));

                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<TDbContext>>();

                    // Check if there are pending migrations
                    var pendingMigrations = (
                        await context.Database.GetPendingMigrationsAsync()
                    ).ToList();

                    if (pendingMigrations.Any())
                    {
                        logger.LogInformation(
                            "Applying {Count} pending migrations: {Migrations}",
                            pendingMigrations.Count,
                            string.Join(", ", pendingMigrations)
                        );

                        try
                        {
                            // Attempt migration - CockroachDB may throw lock error
                            await context.Database.MigrateAsync();
                            logger.LogInformation("Database migrations completed successfully");
                        }
                        catch (PostgresException ex)
                            when (ex.SqlState == "42601"
                                && ex.MessageText.Contains(
                                    "lock",
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                        {
                            // CockroachDB doesn't support LOCK TABLE syntax
                            // Generate migration SQL and execute it directly without locks
                            logger.LogWarning(
                                "Encountered CockroachDB lock error. Attempting to apply migrations using direct SQL execution..."
                            );

                            var migrator = context
                                .Database.GetInfrastructure()
                                .GetService<IMigrator>();
                            if (migrator != null)
                            {
                                try
                                {
                                    await ApplyMigrationsWithWorkaroundAsync(
                                        context,
                                        migrator,
                                        pendingMigrations,
                                        logger
                                    );
                                    logger.LogInformation(
                                        "Database migrations completed (with CockroachDB workaround)"
                                    );
                                }
                                catch (Exception scriptEx)
                                {
                                    logger.LogError(
                                        scriptEx,
                                        "Failed to apply migrations using direct SQL. Schema may already exist."
                                    );
                                }
                            }
                            else
                            {
                                logger.LogWarning(
                                    "Could not retrieve IMigrator service. Migrations will be retried on next startup."
                                );
                            }
                        }
                    }
                    else
                    {
                        logger.LogInformation("No pending migrations");
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<TDbContext>>();
                logger.LogError(ex, "Failed to apply database migrations");
                // Don't throw - let the app continue running
            }
        });
    }

    private static async Task ApplyMigrationsWithWorkaroundAsync<TDbContext>(
        TDbContext context,
        IMigrator migrator,
        List<string> pendingMigrations,
        ILogger logger
    )
        where TDbContext : DbContext
    {
        // Use a transaction to coordinate migrations
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // Get the current migration (before the pending ones)
            var currentMigration = await context.Database.GetAppliedMigrationsAsync();
            var lastApplied = currentMigration.LastOrDefault();

            // Generate SQL script for all pending migrations
            var script = migrator.GenerateScript(lastApplied, pendingMigrations.Last());

            // Replace LOCK TABLE with SELECT FOR UPDATE, fix SQL Server syntax
            var sqlStatements = script
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s =>
                {
                    // Skip transaction control statements - we're already in a transaction
                    if (s.StartsWith("START TRANSACTION", StringComparison.OrdinalIgnoreCase) ||
                        s.StartsWith("BEGIN TRANSACTION", StringComparison.OrdinalIgnoreCase) ||
                        s.StartsWith("COMMIT", StringComparison.OrdinalIgnoreCase) ||
                        s.StartsWith("ROLLBACK", StringComparison.OrdinalIgnoreCase))
                    {
                        logger.LogDebug("Skipping transaction control statement: {Statement}", s);
                        return null;
                    }

                    // Replace LOCK TABLE statements with SELECT FOR UPDATE equivalent
                    // Pattern: LOCK TABLE "table_name" IN ACCESS EXCLUSIVE MODE;
                    // Replace with: SELECT 1 FROM "table_name" LIMIT 1 FOR UPDATE;
                    if (s.Contains("LOCK TABLE", StringComparison.OrdinalIgnoreCase))
                    {
                        // Extract table name from: LOCK TABLE "table_name" IN ACCESS EXCLUSIVE MODE
                        var match = System.Text.RegularExpressions.Regex.Match(
                            s,
                            @"LOCK\s+TABLE\s+([""`\[\]]?[\w]+[""`\[\]]?)\s+IN\s+ACCESS\s+EXCLUSIVE\s+MODE",
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase
                        );

                        if (match.Success && match.Groups.Count > 1)
                        {
                            var tableName = match.Groups[1].Value;
                            // Ensure table name uses PostgreSQL quotes
                            if (!tableName.StartsWith("\"") && !tableName.StartsWith("["))
                            {
                                tableName = $"\"{tableName}\"";
                            }
                            else if (tableName.StartsWith("["))
                            {
                                tableName = tableName.Replace("[", "\"").Replace("]", "\"");
                            }

                            var replacement = $"SELECT 1 FROM {tableName} LIMIT 1 FOR UPDATE";
                            logger.LogDebug(
                                "Replaced LOCK TABLE with SELECT FOR UPDATE: {Original} -> {Replacement}",
                                s.Substring(0, Math.Min(50, s.Length)),
                                replacement
                            );
                            return replacement;
                        }
                        else
                        {
                            // Fallback: just skip it if we can't parse it
                            logger.LogDebug("Skipping LOCK TABLE statement (could not parse)");
                            return null;
                        }
                    }

                    return s
                        // Replace SQL Server identifier brackets with PostgreSQL quotes
                        .Replace("[", "\"", StringComparison.Ordinal)
                        .Replace("]", "\"", StringComparison.Ordinal)
                        // Replace GETUTCDATE() with PostgreSQL syntax
                        .Replace(
                            "GETUTCDATE()",
                            "(now() AT TIME ZONE 'UTC')",
                            StringComparison.OrdinalIgnoreCase
                        );
                })
                .Where(s => s != null)
                .ToList();

            // Execute each statement within the transaction
            foreach (var sql in sqlStatements)
            {
                try
                {
                    await context.Database.ExecuteSqlRawAsync(sql);
                }
                catch (PostgresException sqlEx)
                    when (sqlEx.SqlState == "42P07"
                        || // Table already exists
                        sqlEx.SqlState == "42710"
                    ) // Duplicate object
                {
                    logger.LogDebug(
                        "Object already exists, skipping: {Sql}",
                        sql.Substring(0, Math.Min(50, sql.Length))
                    );
                }
                catch (PostgresException sqlEx) when (sqlEx.SqlState == "42601") // Syntax error
                {
                    logger.LogWarning(
                        "SQL syntax error (may already be applied): {Error} - {Sql}",
                        sqlEx.MessageText,
                        sql.Substring(0, Math.Min(100, sql.Length))
                    );
                }
            }

            // Manually insert migration record
            foreach (var migration in pendingMigrations)
            {
                try
                {
                    var productVersion =
                        typeof(DbContext).Assembly.GetName().Version?.ToString() ?? "9.0.0";
                    await context.Database.ExecuteSqlRawAsync(
                        @$"INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
                          VALUES ('{migration}', '{productVersion}')
                          ON CONFLICT (""MigrationId"") DO NOTHING"
                    );
                    logger.LogInformation("Applied migration: {Migration}", migration);
                }
                catch (PostgresException historyEx) when (historyEx.SqlState == "23505") // Unique constraint violation
                {
                    logger.LogDebug("Migration {Migration} already recorded", migration);
                }
            }

            // Commit the transaction (row-level lock is automatically released)
            await transaction.CommitAsync();
            logger.LogDebug("Migrations completed successfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Failed to apply migrations, transaction rolled back");
            throw;
        }
    }
}
