// Data/ApplicationDbContext.cs
using FindTradie.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace FindTradie.Shared.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // DbSets will be added as we create entities
    // public DbSet<User> Users { get; set; }
    // public DbSet<TradieProfile> TradieProfiles { get; set; }
    // public DbSet<Job> Jobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
                var lambda = Expression.Lambda(notDeleted, param);   // LambdaExpression

                modelBuilder.Entity(clr).HasQueryFilter(lambda);         // EF Core 8/9 overload
            }
        }
    }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
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

        return base.SaveChangesAsync(cancellationToken);
    }
}