using Account.Service.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Pogo.Shared.Infrastructure;
using AccountEntity = Account.Service.Domain.Entities.Account;

namespace Account.Service.Infrastructure.Data;

/// <summary>
/// DbContext for Account microservice
/// </summary>
public class AccountDbContext : BaseDbContext
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
    }

    public DbSet<AccountEntity> Accounts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Account entity
        modelBuilder.Entity<AccountEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PlayerId).IsRequired();
            entity.Property(e => e.DateJoined).IsRequired();
            entity.Property(e => e.WrongAttempts).HasDefaultValue(0);
            entity.Property(e => e.LockedOut).IsRequired(false);

            // Create unique index on email
            entity.HasIndex(e => e.Email).IsUnique();
            
            // Create index on player ID
            entity.HasIndex(e => e.PlayerId);
        });
    }
}
