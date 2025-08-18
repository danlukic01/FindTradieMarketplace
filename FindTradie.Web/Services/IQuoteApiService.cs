// Services/IQuoteApiService.cs
using FindTradie.Services.JobManagement.DTOs;
using FindTradie.Shared.Contracts.Common;

namespace FindTradie.Web.Services;

public interface IQuoteApiService
{
    Task<ApiResponse<QuoteDetailDto>> CreateQuoteAsync(CreateQuoteRequest request);
    Task<ApiResponse<QuoteDetailDto>> GetQuoteAsync(Guid id);
    Task<ApiResponse<List<QuoteSummaryDto>>> GetQuotesByJobAsync(Guid jobId);
    Task<ApiResponse<List<QuoteSummaryDto>>> GetTradieQuotesAsync(Guid tradieId, int pageNumber = 1, int pageSize = 20);
    Task<ApiResponse<bool>> UpdateQuoteStatusAsync(Guid id, UpdateQuoteStatusRequest request);
    Task<ApiResponse<bool>> WithdrawQuoteAsync(Guid id, string reason);
}