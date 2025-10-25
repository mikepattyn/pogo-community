using Gym.Service.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Pogo.Shared.Infrastructure;
using GymEntity = Gym.Service.Domain.Entities.Gym;

namespace Gym.Service.Infrastructure.Data;

/// <summary>
/// DbContext for Gym microservice
/// </summary>
public class GymDbContext : BaseDbContext
{
    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
    {
    }

    public DbSet<GymEntity> Gyms { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Gym entity
        modelBuilder.Entity<GymEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.LocationId).IsRequired();
            entity.Property(e => e.Level).IsRequired();
            entity.Property(e => e.ControllingTeam).HasMaxLength(20);
            entity.Property(e => e.IsUnderAttack).IsRequired();
            entity.Property(e => e.IsInRaid).IsRequired();
            entity.Property(e => e.MotivationLevel).IsRequired();
            entity.Property(e => e.LastUpdated).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(1000);

            // Create indexes for efficient searching
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.LocationId);
            entity.HasIndex(e => e.ControllingTeam);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsUnderAttack);
            entity.HasIndex(e => e.IsInRaid);
            entity.HasIndex(e => e.LastUpdated);
        });
    }
}
