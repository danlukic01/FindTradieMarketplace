// ===== REPOSITORIES =====
// Repositories/ITradieRepository.cs
using FindTradie.Services.TradieManagement.Entities;
using FindTradie.Services.TradieManagement.DTOs;

namespace FindTradie.Services.TradieManagement.Repositories;

public interface ITradieRepository
{
    Task<TradieProfile?> GetByIdAsync(Guid id);
    Task<TradieProfile?> GetByUserIdAsync(Guid userId);
    Task<List<TradieProfileSummaryDto>> SearchTradiesAsync(SearchTradiesRequest request);
    Task<TradieProfile> CreateAsync(TradieProfile profile);
    Task<TradieProfile> UpdateAsync(TradieProfile profile);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> IsABNUniqueAsync(string abn, Guid? excludeId = null);
    Task<List<TradieProfile>> GetAvailableTradiesAsync(double latitude, double longitude, double radiusKm);
}