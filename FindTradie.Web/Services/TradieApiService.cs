// Services/TradieApiService.cs
using FindTradie.Services.TradieManagement.DTOs;
using FindTradie.Shared.Contracts.Common;
using System.Text.Json;
using System.Text;

namespace FindTradie.Web.Services;

public class TradieApiService : ITradieApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly ILogger<TradieApiService> _logger;

    public TradieApiService(IHttpClientFactory httpClientFactory, IAuthService authService, ILogger<TradieApiService> logger)
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

    public async Task<ApiResponse<TradieProfileDetailDto>> CreateProfileAsync(CreateTradieProfileRequest request)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/tradies", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<TradieProfileDetailDto>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ApiResponse<TradieProfileDetailDto> { Success = false, Message = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tradie profile");
            return new ApiResponse<TradieProfileDetailDto>
            {
                Success = false,
                Message = "An error occurred while creating the profile",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<TradieProfileDetailDto>> GetProfileAsync(Guid id)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"/api/tradies/{id}");
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<TradieProfileDetailDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ApiResponse<TradieProfileDetailDto> { Success = false, Message = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tradie profile {Id}", id);
            return new ApiResponse<TradieProfileDetailDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the profile",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<TradieProfileDetailDto>> GetProfileByUserIdAsync(Guid userId)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"/api/tradies/user/{userId}");
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<TradieProfileDetailDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ApiResponse<TradieProfileDetailDto> { Success = false, Message = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tradie profile for user {UserId}", userId);
            return new ApiResponse<TradieProfileDetailDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the profile",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<TradieProfileDetailDto>> UpdateProfileAsync(Guid id, UpdateTradieProfileRequest request)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/tradies/{id}", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<TradieProfileDetailDto>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ApiResponse<TradieProfileDetailDto> { Success = false, Message = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tradie profile {Id}", id);
            return new ApiResponse<TradieProfileDetailDto>
            {
                Success = false,
                Message = "An error occurred while updating the profile",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<List<TradieProfileSummaryDto>>> SearchTradiesAsync(SearchTradiesRequest request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/tradies/search", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<List<TradieProfileSummaryDto>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ApiResponse<List<TradieProfileSummaryDto>> { Success = false, Message = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tradies");
            return new ApiResponse<List<TradieProfileSummaryDto>>
            {
                Success = false,
                Message = "An error occurred while searching tradies",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> UpdateAvailabilityAsync(Guid id, bool isAvailable, DateTime? availableFrom = null)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var request = new { IsAvailable = isAvailable, AvailableFrom = availableFrom };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"/api/tradies/{id}/availability", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ApiResponse<bool> { Success = false, Message = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tradie availability {Id}", id);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while updating availability",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}