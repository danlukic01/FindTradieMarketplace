// Services/IQuoteService.cs
using FindTradie.Services.JobManagement.DTOs;
using FindTradie.Shared.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FindTradie.Services.JobManagement.Services;

public interface IQuoteService
{
    Task<ApiResponse<QuoteDetailDto>> CreateQuoteAsync(CreateQuoteRequest request);
    Task<ApiResponse<QuoteDetailDto>> GetQuoteAsync(Guid id);
    Task<ApiResponse<List<QuoteSummaryDto>>> GetQuotesByJobAsync(Guid jobId);
    Task<ApiResponse<List<QuoteSummaryDto>>> GetQuotesByTradieAsync(Guid tradieId, int pageNumber = 1, int pageSize = 20);
    Task<ApiResponse<bool>> UpdateQuoteStatusAsync(Guid id, UpdateQuoteStatusRequest request);
    Task<ApiResponse<bool>> WithdrawQuoteAsync(Guid id, string reason);
}