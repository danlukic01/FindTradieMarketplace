// Data/JobDbContext.cs
using Microsoft.EntityFrameworkCore;
using FindTradie.Services.JobManagement.Entities;
using FindTradie.Shared.Domain.Entities;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq.Expressions;

namespace FindTradie.Services.JobManagement.Data;

public class JobDbContext : DbContext
{
    public JobDbContext(DbContextOptions<JobDbContext> options) : base(options) { }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<Quote> Quotes { get; set; }
    public DbSet<QuoteItem> QuoteItems { get; set; }
    public DbSet<JobImage> JobImages { get; set; }
    public DbSet<JobMessage> JobMessages { get; set; }
    public DbSet<JobStatusHistory> JobStatusHistory { get; set; }
    public DbSet<JobPayment> JobPayments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Job entity
        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.SubCategory).HasMaxLength(100);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CustomerPhone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Suburb).IsRequired().HasMaxLength(100);
            entity.Property(e => e.State).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PostCode).IsRequired().HasMaxLength(10);

            // If Latitude/Longitude are double, remove HasPrecision (it's for decimal).
            // entity.Property(e => e.Latitude).HasPrecision(10, 8);
            // entity.Property(e => e.Longitude).HasPrecision(11, 8);

            entity.Property(e => e.BudgetMin).HasPrecision(12, 2);
            entity.Property(e => e.BudgetMax).HasPrecision(12, 2);
            entity.Property(e => e.FinalAmount).HasPrecision(12, 2);
            entity.Property(e => e.SpecialRequirements).HasMaxLength(1000);
            entity.Property(e => e.CompletionNotes).HasMaxLength(1000);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.AssignedTradieId);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Urgency);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.Latitude, e.Longitude });
            entity.HasIndex(e => e.PostCode);

            entity.HasMany(e => e.Images)
                  .WithOne(e => e.Job)
                  .HasForeignKey(e => e.JobId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Quotes)
                  .WithOne(e => e.Job)
                  .HasForeignKey(e => e.JobId)
                  .OnDelete(DeleteBehavior.Cascade);   // keep cascade here

            entity.HasMany(e => e.Messages)
                  .WithOne(e => e.Job)
                  .HasForeignKey(e => e.JobId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.StatusHistory)
                  .WithOne(e => e.Job)
                  .HasForeignKey(e => e.JobId)
                  .OnDelete(DeleteBehavior.Cascade);

            // BREAK the cycle: do NOT cascade from Job -> AcceptedQuote
            entity.HasOne(e => e.AcceptedQuote)
                  .WithMany()
                  .HasForeignKey(e => e.AcceptedQuoteId)
                  .OnDelete(DeleteBehavior.NoAction);  // was SetNull
        });

        // Configure Quote entity
        modelBuilder.Entity<Quote>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TradieBusinessName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.MaterialsCost).HasPrecision(12, 2);
            entity.Property(e => e.LabourCost).HasPrecision(12, 2);
            entity.Property(e => e.TotalCost).HasPrecision(12, 2);
            entity.Property(e => e.PricingBreakdown).HasMaxLength(2000);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.MaterialsIncluded).HasMaxLength(1000);
            entity.Property(e => e.Methodology).HasMaxLength(1000);
            entity.Property(e => e.WarrantyOffered).HasMaxLength(500);
            entity.Property(e => e.CustomerNotes).HasMaxLength(1000);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);

            entity.HasIndex(e => e.JobId);
            entity.HasIndex(e => e.TradieId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.ExpiresAt);

            entity.HasMany(e => e.Items)
                  .WithOne(e => e.Quote)
                  .HasForeignKey(e => e.QuoteId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure QuoteItem entity
        modelBuilder.Entity<QuoteItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.UnitPrice).HasPrecision(12, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(12, 2);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasIndex(e => e.QuoteId);
        });

        // Configure JobImage entity
        modelBuilder.Entity<JobImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImageUrl)
                  .IsRequired()
                  .HasColumnType("nvarchar(max)");
            entity.Property(e => e.Caption).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasIndex(e => e.JobId);
            entity.HasIndex(e => e.ImageType);
        });

        // Configure JobMessage entity
        modelBuilder.Entity<JobMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SenderName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.AttachmentUrl).HasMaxLength(500);

            entity.HasIndex(e => e.JobId);
            entity.HasIndex(e => e.SenderId);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Configure JobStatusHistory entity
        modelBuilder.Entity<JobStatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.ChangedByName).IsRequired().HasMaxLength(100);

            entity.HasIndex(e => e.JobId);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Configure JobPayment entity
        modelBuilder.Entity<JobPayment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PaymentIntentId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Amount).HasPrecision(12, 2);
            entity.Property(e => e.PlatformFee).HasPrecision(12, 2);
            entity.Property(e => e.TradieAmount).HasPrecision(12, 2);
            entity.Property(e => e.PaymentMethodId).HasMaxLength(100);
            entity.Property(e => e.FailureReason).HasMaxLength(500);
            entity.Property(e => e.ReceiptUrl).HasMaxLength(500);

            entity.HasIndex(e => e.JobId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.PaymentIntentId).IsUnique();
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
