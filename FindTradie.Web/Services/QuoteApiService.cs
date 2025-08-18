// Services/QuoteApiService.cs
using FindTradie.Services.JobManagement.DTOs;
using FindTradie.Shared.Contracts.Common;
using System.Text.Json;
using System.Text;

namespace FindTradie.Web.Services;

public class QuoteApiService : IQuoteApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly ILogger<QuoteApiService> _logger;

    public QuoteApiService(IHttpClientFactory httpClientFactory, IAuthService authService, ILogger<QuoteApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("FindTradieAPI");
        _authService = authService;
        _logger = logger;
    }

    private async Task SetAuthorizationHeaderAsync()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<ApiResponse<QuoteDetailDto>> CreateQuoteAsync(CreateQuoteRequest request)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/quotes", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<QuoteDetailDto>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ApiResponse<QuoteDetailDto> { Success = false, Message = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quote");
            return new ApiResponse<QuoteDetailDto>
            {
                Success = false,
                Message = "An error occurred while creating the quote",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<QuoteDetailDto>> GetQuoteAsync(Guid id)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"/api/quotes/{id}");
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<QuoteDetailDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ApiResponse<QuoteDetailDto> { Success = false, Message = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quote {QuoteId}", id);
            return new ApiResponse<QuoteDetailDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the quote",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<List<QuoteSummaryDto>>> GetQuotesByJobAsync(Guid jobId)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"/api/quotes/job/{jobId}");
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<List<QuoteSummaryDto>>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ApiResponse<List<QuoteSummaryDto>> { Success = false, Message = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quotes for job {JobId}", jobId);
            return new ApiResponse<List<QuoteSummaryDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving quotes",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<List<QuoteSummaryDto>>> GetTradieQuotesAsync(Guid tradieId, int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"/api/quotes/tradie/{tradieId}?pageNumber={pageNumber}&pageSize={pageSize}");
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<List<QuoteSummaryDto>>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ApiResponse<List<QuoteSummaryDto>> { Success = false, Message = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tradie quotes for {TradieId}", tradieId);
            return new ApiResponse<List<QuoteSummaryDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving quotes",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> UpdateQuoteStatusAsync(Guid id, UpdateQuoteStatusRequest request)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"/api/quotes/{id}/status", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ApiResponse<bool> { Success = false, Message = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quote status for {QuoteId}", id);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while updating quote status",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> WithdrawQuoteAsync(Guid id, string reason)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var request = new { Reason = reason };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/api/quotes/{id}/withdraw", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ApiResponse<bool> { Success = false, Message = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error withdrawing quote {QuoteId}", id);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while withdrawing the quote",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}