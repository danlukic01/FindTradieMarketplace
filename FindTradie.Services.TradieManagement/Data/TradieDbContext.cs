// ===== DATABASE CONTEXT =====
// Data/TradieDbContext.cs
using Microsoft.EntityFrameworkCore;
using FindTradie.Services.TradieManagement.Entities;
using FindTradie.Shared.Domain.Entities;
using System.Linq.Expressions;       

namespace FindTradie.Services.TradieManagement.Data;

public class TradieDbContext : DbContext
{
    public TradieDbContext(DbContextOptions<TradieDbContext> options) : base(options) { }

    public DbSet<TradieProfile> TradieProfiles { get; set; }
    public DbSet<TradieService> TradieServices { get; set; }
    public DbSet<TradieDocument> TradieDocuments { get; set; }
    public DbSet<PortfolioItem> PortfolioItems { get; set; }
    public DbSet<TradieAvailability> TradieAvailability { get; set; }
    public DbSet<TradieLocation> TradieLocations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure TradieProfile
        modelBuilder.Entity<TradieProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BusinessName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ABN).IsRequired().HasMaxLength(11);
            entity.Property(e => e.ACN).HasMaxLength(9);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.HourlyRate).HasPrecision(10, 2);
            entity.Property(e => e.Rating).HasPrecision(3, 2);
            entity.Property(e => e.InsuranceProvider).HasMaxLength(200);
            entity.Property(e => e.InsurancePolicyNumber).HasMaxLength(100);

            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasIndex(e => e.ABN).IsUnique();
            entity.HasIndex(e => e.VerificationStatus);
            entity.HasIndex(e => e.IsAvailable);
            entity.HasIndex(e => e.Rating);

            // Configure relationships
            entity.HasMany(e => e.Services)
                  .WithOne(e => e.TradieProfile)
                  .HasForeignKey(e => e.TradieProfileId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Documents)
                  .WithOne(e => e.TradieProfile)
                  .HasForeignKey(e => e.TradieProfileId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Portfolio)
                  .WithOne(e => e.TradieProfile)
                  .HasForeignKey(e => e.TradieProfileId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Availability)
                  .WithOne(e => e.TradieProfile)
                  .HasForeignKey(e => e.TradieProfileId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.ServiceLocations)
                  .WithOne(e => e.TradieProfile)
                  .HasForeignKey(e => e.TradieProfileId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure TradieService
        modelBuilder.Entity<TradieService>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SubCategory).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.MinPrice).HasPrecision(10, 2);
            entity.Property(e => e.MaxPrice).HasPrecision(10, 2);

            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsActive);
        });

        // Configure TradieDocument
        modelBuilder.Entity<TradieDocument>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocumentName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DocumentUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.VerificationNotes).HasMaxLength(1000);
            entity.Property(e => e.VerifiedBy).HasMaxLength(100);

            entity.HasIndex(e => e.DocumentType);
            entity.HasIndex(e => e.VerificationStatus);
        });

        // Configure PortfolioItem
        modelBuilder.Entity<PortfolioItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.BeforeImageUrl).HasMaxLength(500);
            entity.Property(e => e.AfterImageUrl).HasMaxLength(500);
            entity.Property(e => e.ProjectValue).HasPrecision(12, 2);

            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsPublic);
            entity.HasIndex(e => e.CompletionDate);
        });

        // Configure TradieAvailability
        modelBuilder.Entity<TradieAvailability>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.TradieProfileId, e.DayOfWeek }).IsUnique();
        });

        // Configure TradieLocation
        modelBuilder.Entity<TradieLocation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Suburb).IsRequired().HasMaxLength(100);
            entity.Property(e => e.State).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PostCode).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Latitude).HasPrecision(10, 8);
            entity.Property(e => e.Longitude).HasPrecision(11, 8);

            entity.HasIndex(e => e.PostCode);
            entity.HasIndex(e => new { e.Latitude, e.Longitude });
        });

        // Global query filter for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;
            if (typeof(BaseEntity).IsAssignableFrom(clr))
            {
                // Build: (e) => !((BaseEntity)e).IsDeleted
                var param = Expression.Parameter(clr, "e");
                var cast = Expression.Convert(param, typeof(BaseEntity));
                var isDeleted = Expression.Property(cast, nameof(BaseEntity.IsDeleted));
                var notDeleted = Expression.Equal(isDeleted, Expression.Constant(false));
                var lambda = Expression.Lambda(notDeleted, param);     // LambdaExpression

                modelBuilder.Entity(clr).HasQueryFilter(lambda);           // non-generic overload
            }
        }
        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed service categories data could go here if needed
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}