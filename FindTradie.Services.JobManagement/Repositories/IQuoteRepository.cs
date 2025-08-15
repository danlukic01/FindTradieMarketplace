// Repositories/IQuoteRepository.cs
using FindTradie.Services.JobManagement.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FindTradie.Services.JobManagement.Repositories;

public interface IQuoteRepository
{
    Task<Quote?> GetByIdAsync(Guid id);
    Task<Quote?> GetByIdWithDetailsAsync(Guid id);
    Task<List<Quote>> GetQuotesByJobAsync(Guid jobId);
    Task<List<Quote>> GetQuotesByTradieAsync(Guid tradieId, int pageNumber = 1, int pageSize = 20);
    Task<Quote> CreateAsync(Quote quote);
    Task<Quote> UpdateAsync(Quote quote);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> HasTradieQuotedForJobAsync(Guid tradieId, Guid jobId);
    Task<List<Quote>> GetExpiredQuotesAsync();
}