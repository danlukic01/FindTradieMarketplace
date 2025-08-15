// ===== TRADIE API SERVICE =====
// Services/ITradieApiService.cs
using FindTradie.Services.TradieManagement.DTOs;
using FindTradie.Shared.Contracts.Common;

namespace FindTradie.Web.Services;

public interface ITradieApiService
{
    Task<ApiResponse<TradieProfileDetailDto>> CreateProfileAsync(CreateTradieProfileRequest request);
    Task<ApiResponse<TradieProfileDetailDto>> GetProfileAsync(Guid id);
    Task<ApiResponse<TradieProfileDetailDto>> GetProfileByUserIdAsync(Guid userId);
    Task<ApiResponse<TradieProfileDetailDto>> UpdateProfileAsync(Guid id, UpdateTradieProfileRequest request);
    Task<ApiResponse<List<TradieProfileSummaryDto>>> SearchTradiesAsync(SearchTradiesRequest request);
    Task<ApiResponse<bool>> UpdateAvailabilityAsync(Guid id, bool isAvailable, DateTime? availableFrom = null);
}