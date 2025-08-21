// Repositories/QuoteRepository.cs
using Microsoft.EntityFrameworkCore;
using FindTradie.Services.JobManagement.Data;
using FindTradie.Services.JobManagement.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FindTradie.Services.JobManagement.Repositories;

public class QuoteRepository : IQuoteRepository
{
    private readonly JobDbContext _context;

    public QuoteRepository(JobDbContext context)
    {
        _context = context;
    }

    public async Task<Quote?> GetByIdAsync(Guid id)
    {
        return await _context.Quotes.FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<Quote?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Quotes
            .Include(q => q.Items)
            .Include(q => q.Job)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<List<Quote>> GetQuotesByJobAsync(Guid jobId)
    {
        return await _context.Quotes
            .Include(q => q.Items)
            .Where(q => q.JobId == jobId)
            .OrderBy(q => q.TotalCost)
            .ToListAsync();
    }

    public async Task<List<Quote>> GetQuotesByTradieAsync(Guid tradieId, int pageNumber = 1, int pageSize = 20)
    {
        return await _context.Quotes
            .Include(q => q.Job)
            .Where(q => q.TradieId == tradieId)
            .OrderByDescending(q => q.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Quote> CreateAsync(Quote quote)
    {
        _context.Quotes.Add(quote);
        await _context.SaveChangesAsync();
        return quote;
    }

    public async Task<Quote> UpdateAsync(Quote quote)
    {
        // Similar to jobs, quotes and their related items are tracked by the
        // context. Using Update would mark new child entities as modified and
        // lead to failed updates for non-existent rows. Saving changes directly
        // ensures EF Core inserts new records and updates existing ones
        // appropriately.
        await _context.SaveChangesAsync();
        return quote;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Quotes.AnyAsync(q => q.Id == id);
    }

    public async Task<bool> HasTradieQuotedForJobAsync(Guid tradieId, Guid jobId)
    {
        return await _context.Quotes.AnyAsync(q => q.TradieId == tradieId && q.JobId == jobId);
    }

    public async Task<List<Quote>> GetExpiredQuotesAsync()
    {
        return await _context.Quotes
            .Where(q => q.ExpiresAt < DateTime.UtcNow && q.Status == QuoteStatus.Submitted)
            .ToListAsync();
    }
}