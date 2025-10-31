using Location.Service.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Pogo.Shared.Infrastructure;
using LocationEntity = Location.Service.Domain.Entities.Location;

namespace Location.Service.Infrastructure.Data;

/// <summary>
/// DbContext for Location microservice
/// </summary>
public class LocationDbContext : BaseDbContext
{
    public LocationDbContext(DbContextOptions<LocationDbContext> options) : base(options)
    {
    }

    public DbSet<LocationEntity> Locations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Location entity
        modelBuilder.Entity<LocationEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Latitude).IsRequired();
            entity.Property(e => e.Longitude).IsRequired();
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.LocationType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            // Create indexes for efficient searching
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.LocationType);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.City);
            entity.HasIndex(e => e.Country);

            // Create spatial index for location-based queries
            entity.HasIndex(e => new { e.Latitude, e.Longitude });

            entity.ToTable("Locations");
        });
    }


}
