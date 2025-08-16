// Services/IUserService.cs
using FindTradie.Services.UserManagement.DTOs;
using FindTradie.Shared.Contracts.Common;
using FindTradie.Shared.Contracts.DTOs;

namespace FindTradie.Services.UserManagement.Services;

public interface IUserService
{
    Task<ApiResponse<UserProfileDto>> CreateUserAsync(CreateUserRequest request);
    Task<ApiResponse<UserProfileDto>> GetUserAsync(Guid id);
    Task<ApiResponse<UserProfileDto>> GetUserByEmailAsync(string email);
    Task<ApiResponse<UserProfileDto>> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task<ApiResponse<List<UserProfileDto>>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 20);
    Task<ApiResponse<bool>> DeleteUserAsync(Guid id);
    Task<ApiResponse<bool>> VerifyEmailAsync(Guid id);
    Task<ApiResponse<bool>> VerifyPhoneAsync(Guid id);
    Task<ApiResponse<string>> LoginAsync(string email, string password);
}