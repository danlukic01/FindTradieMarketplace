// Services/IUserApiService.cs
using FindTradie.Shared.Contracts.DTOs;
using FindTradie.Shared.Contracts.Common;
using FindTradie.Services.UserManagement.DTOs;

namespace FindTradie.Web.Services;

public interface IUserApiService
{
    Task<ApiResponse<UserProfileDto>> GetUserAsync(Guid id);
    Task<ApiResponse<UserProfileDto>> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task<ApiResponse<List<UserProfileDto>>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 20);
}