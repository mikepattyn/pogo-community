using Microsoft.EntityFrameworkCore;
using Pogo.Shared.Infrastructure;
using RaidEntity = Raid.Service.Domain.Entities.Raid;

namespace Raid.Service.Infrastructure.Data;

/// <summary>
/// DbContext for Raid microservice
/// </summary>
public class RaidDbContext : BaseDbContext
{
    public RaidDbContext(DbContextOptions<RaidDbContext> options) : base(options)
    {
    }

    public DbSet<RaidEntity> Raids { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Raid entity
        modelBuilder.Entity<RaidEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DiscordMessageId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.GymId).IsRequired();
            entity.Property(e => e.PokemonSpecies).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Level).IsRequired();
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.EndTime).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.IsCompleted).IsRequired();
            entity.Property(e => e.IsCancelled).IsRequired();
            entity.Property(e => e.MaxParticipants).IsRequired();
            entity.Property(e => e.CurrentParticipants).IsRequired();
            entity.Property(e => e.Difficulty).IsRequired().HasMaxLength(50);
            entity.Property(e => e.WeatherBoost).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            // Create indexes for efficient searching
            entity.HasIndex(e => e.DiscordMessageId).IsUnique();
            entity.HasIndex(e => e.GymId);
            entity.HasIndex(e => e.PokemonSpecies);
            entity.HasIndex(e => e.Level);
            entity.HasIndex(e => e.StartTime);
            entity.HasIndex(e => e.EndTime);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsCompleted);
            entity.HasIndex(e => e.IsCancelled);
            entity.HasIndex(e => e.Difficulty);

            // Composite indexes for common queries
            entity.HasIndex(e => new { e.GymId, e.StartTime });
            entity.HasIndex(e => new { e.IsActive, e.StartTime });
            entity.HasIndex(e => new { e.Level, e.IsActive });
        });
    }
}
