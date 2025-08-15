// Services/IAuthService.cs
using FindTradie.Shared.Contracts.DTOs;
using FindTradie.Shared.Contracts.Common;

namespace FindTradie.Web.Services;

public interface IAuthService
{
    Task<ApiResponse<string>> LoginAsync(string email, string password);
    Task<ApiResponse<UserProfileDto>> RegisterAsync(CreateUserRequest request);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task<UserProfileDto?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
}