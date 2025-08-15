// ===== SERVICES =====
// Services/ITradieService.cs
using FindTradie.Services.TradieManagement.DTOs;
using FindTradie.Shared.Contracts.Common;
using FindTradie.Shared.Domain.Enums;

namespace FindTradie.Services.TradieManagement.Services;

public interface ITradieService
{
    Task<ApiResponse<TradieProfileDetailDto>> CreateProfileAsync(CreateTradieProfileRequest request);
    Task<ApiResponse<TradieProfileDetailDto>> GetProfileAsync(Guid id);
    Task<ApiResponse<TradieProfileDetailDto>> GetProfileByUserIdAsync(Guid userId);
    Task<ApiResponse<TradieProfileDetailDto>> UpdateProfileAsync(Guid id, UpdateTradieProfileRequest request);
    Task<ApiResponse<List<TradieProfileSummaryDto>>> SearchTradiesAsync(SearchTradiesRequest request);
    Task<ApiResponse<bool>> UpdateAvailabilityAsync(Guid id, bool isAvailable, DateTime? availableFrom = null);
    Task<ApiResponse<bool>> VerifyTradieAsync(Guid id, VerificationStatus status, string? notes = null);
}