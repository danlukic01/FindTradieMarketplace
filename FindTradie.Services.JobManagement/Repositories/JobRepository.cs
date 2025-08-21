// Repositories/JobRepository.cs
using Microsoft.EntityFrameworkCore;
using FindTradie.Services.JobManagement.Data;
using FindTradie.Services.JobManagement.Entities;
using FindTradie.Services.JobManagement.DTOs;
using FindTradie.Shared.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace FindTradie.Services.JobManagement.Repositories;

public class JobRepository : IJobRepository
{
    private readonly JobDbContext _context;

    public JobRepository(JobDbContext context)
    {
        _context = context;
    }

    public async Task<Job?> GetByIdAsync(Guid id)
    {
        return await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<Job?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Jobs
            .Include(j => j.Images.OrderBy(i => i.DisplayOrder))
            .Include(j => j.Quotes.Where(q => !q.IsDeleted))
                .ThenInclude(q => q.Items)
            .Include(j => j.Messages.Where(m => !m.IsDeleted).OrderByDescending(m => m.CreatedAt).Take(10))
            .Include(j => j.StatusHistory.OrderByDescending(h => h.CreatedAt))
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<List<JobSummaryDto>> SearchJobsAsync(JobSearchRequest request)
    {
        var query = _context.Jobs.AsQueryable();

        // Filter by categories
        if (request.Categories?.Any() == true)
        {
            query = query.Where(j => request.Categories.Contains(j.Category));
        }

        // Filter by urgency
        if (request.Urgency.HasValue)
        {
            query = query.Where(j => j.Urgency == request.Urgency.Value);
        }

        // Filter by budget
        if (request.MinBudget.HasValue)
        {
            query = query.Where(j => j.BudgetMax >= request.MinBudget.Value || j.BudgetMax == null);
        }

        if (request.MaxBudget.HasValue)
        {
            query = query.Where(j => j.BudgetMin <= request.MaxBudget.Value || j.BudgetMin == null);
        }

        // Filter by start date
        if (request.StartDateFrom.HasValue)
        {
            query = query.Where(j => j.PreferredStartDate >= request.StartDateFrom.Value || j.PreferredStartDate == null);
        }

        if (request.StartDateTo.HasValue)
        {
            query = query.Where(j => j.PreferredStartDate <= request.StartDateTo.Value || j.PreferredStartDate == null);
        }

        // Filter by budget availability
        if (request.HasBudget == true)
        {
            query = query.Where(j => j.BudgetMin.HasValue || j.BudgetMax.HasValue);
        }

        // Only show active jobs
        query = query.Where(j => j.Status == JobStatus.Posted || j.Status == JobStatus.QuoteRequested);

        var jobs = await query
            .Select(j => new JobSummaryDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description.Length > 200 ? j.Description.Substring(0, 200) + "..." : j.Description,
                Category = j.Category,
                SubCategory = j.SubCategory,
                Urgency = j.Urgency,
                Status = j.Status,
                BudgetMin = j.BudgetMin,
                BudgetMax = j.BudgetMax,
                Suburb = j.Suburb,
                State = j.State,
                DistanceKm = request.Latitude.HasValue && request.Longitude.HasValue
                    ? CalculateDistance(request.Latitude.Value, request.Longitude.Value, j.Latitude, j.Longitude)
                    : 0,
                CreatedAt = j.CreatedAt,
                QuoteCount = j.Quotes.Count(q => !q.IsDeleted),
                HasImages = j.Images.Any(),
                PreferredStartDate = j.PreferredStartDate
            })
            .Where(j => !request.RadiusKm.HasValue || j.DistanceKm <= request.RadiusKm.Value)
            .OrderBy(j => j.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return jobs;
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in kilometers

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    private static double ToRadians(double degrees) => degrees * (Math.PI / 180);

    public async Task<List<Job>> GetJobsByCustomerAsync(Guid customerId, int pageNumber = 1, int pageSize = 20)
    {
        return await _context.Jobs
            .Include(j => j.Quotes)
            .Include(j => j.Images)
            .Where(j => j.CustomerId == customerId)
            .OrderByDescending(j => j.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Job>> GetJobsByTradieAsync(Guid tradieId, int pageNumber = 1, int pageSize = 20)
    {
        return await _context.Jobs
            .Include(j => j.Quotes.Where(q => q.TradieId == tradieId))
            .Include(j => j.Images)
            .Where(j => j.Quotes.Any(q => q.TradieId == tradieId && !q.IsDeleted) || j.AssignedTradieId == tradieId)
            .OrderByDescending(j => j.UpdatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Job> CreateAsync(Job job)
    {
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        return job;
    }

    public async Task<Job> UpdateAsync(Job job)
    {
        // The job is already being tracked by the context when retrieved through
        // repository methods such as GetByIdWithDetailsAsync. Calling Update on
        // the tracked entity forces EF Core to mark the entire object graph as
        // modified, which causes new child entities (e.g. newly uploaded images)
        // to be treated as updates rather than inserts. This results in a
        // DbUpdateConcurrencyException when EF attempts to update rows that do
        // not yet exist. Simply saving the context will persist any changes and
        // correctly insert new child records.
        await _context.SaveChangesAsync();
        return job;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Jobs.AnyAsync(j => j.Id == id);
    }

    public async Task<int> GetActiveJobsCountByCustomerAsync(Guid customerId)
    {
        return await _context.Jobs
            .CountAsync(j => j.CustomerId == customerId &&
                           (j.Status == JobStatus.Posted ||
                            j.Status == JobStatus.QuoteRequested ||
                            j.Status == JobStatus.Booked ||
                            j.Status == JobStatus.InProgress));
    }

    public async Task<List<Job>> GetJobsNeedingAttentionAsync()
    {
        var threeDaysAgo = DateTime.UtcNow.AddDays(-3);

        return await _context.Jobs
            .Where(j => (j.Status == JobStatus.Posted && j.CreatedAt < threeDaysAgo && !j.Quotes.Any()) ||
                       (j.Status == JobStatus.QuoteReceived && j.UpdatedAt < threeDaysAgo))
            .ToListAsync();
    }

    public async Task<List<Job>> GetExpiredJobsAsync()
    {
        var oneWeekAgo = DateTime.UtcNow.AddDays(-7);

        return await _context.Jobs
            .Where(j => j.Status == JobStatus.Posted && j.CreatedAt < oneWeekAgo)
            .ToListAsync();
    }
}