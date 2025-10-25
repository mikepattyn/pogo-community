using Player.Service.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Pogo.Shared.Infrastructure;
using PlayerEntity = Player.Service.Domain.Entities.Player;

namespace Player.Service.Infrastructure.Data;

/// <summary>
/// DbContext for Player microservice
/// </summary>
public class PlayerDbContext : BaseDbContext
{
    public PlayerDbContext(DbContextOptions<PlayerDbContext> options) : base(options)
    {
    }

    public DbSet<PlayerEntity> Players { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Player entity
        modelBuilder.Entity<PlayerEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Level).IsRequired();
            entity.Property(e => e.Team).IsRequired().HasMaxLength(20);
            entity.Property(e => e.FriendCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.Timezone).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Language).IsRequired().HasMaxLength(10);
            entity.Property(e => e.DiscordUserId).HasMaxLength(50);
            entity.Property(e => e.LastActivity).IsRequired(false);

            // Create unique index on username
            entity.HasIndex(e => e.Username).IsUnique();
            
            // Create unique index on Discord user ID (nullable)
            entity.HasIndex(e => e.DiscordUserId).IsUnique().HasFilter("[DiscordUserId] IS NOT NULL");
            
            // Create index on team for filtering
            entity.HasIndex(e => e.Team);
            
            // Create index on active status
            entity.HasIndex(e => e.IsActive);
        });
    }
}
