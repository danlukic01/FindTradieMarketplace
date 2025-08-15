// Services/UserApiService.cs
using FindTradie.Shared.Contracts.DTOs;
using FindTradie.Shared.Contracts.Common;
using System.Text.Json;
using FindTradie.Services.UserManagement.DTOs;

namespace FindTradie.Web.Services;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public UserApiService(IHttpClientFactory httpClientFactory, IAuthService authService)
    {
        _httpClient = httpClientFactory.CreateClient("FindTradieAPI");
        _authService = authService;
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

    public async Task<ApiResponse<UserProfileDto>> GetUserAsync(Guid id)
    {
        await SetAuthorizationHeaderAsync();
        var response = await _httpClient.GetAsync($"/api/users/{id}");
        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<ApiResponse<UserProfileDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new ApiResponse<UserProfileDto> { Success = false, Message = "Failed to deserialize response" };
    }

    public async Task<ApiResponse<UserProfileDto>> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        await SetAuthorizationHeaderAsync();
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"/api/users/{id}", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<ApiResponse<UserProfileDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new ApiResponse<UserProfileDto> { Success = false, Message = "Failed to deserialize response" };
    }

    public async Task<ApiResponse<List<UserProfileDto>>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 20)
    {
        await SetAuthorizationHeaderAsync();
        var response = await _httpClient.GetAsync($"/api/users?pageNumber={pageNumber}&pageSize={pageSize}");
        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<ApiResponse<List<UserProfileDto>>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new ApiResponse<List<UserProfileDto>> { Success = false, Message = "Failed to deserialize response" };
    }
}