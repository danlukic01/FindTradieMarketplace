// Repositories/TradieRepository.cs
using Microsoft.EntityFrameworkCore;
using FindTradie.Services.TradieManagement.Data;
using FindTradie.Services.TradieManagement.Entities;
using FindTradie.Services.TradieManagement.DTOs;
using FindTradie.Shared.Domain.Enums;

namespace FindTradie.Services.TradieManagement.Repositories;

public class TradieRepository : ITradieRepository
{
    private readonly TradieDbContext _context;

    public TradieRepository(TradieDbContext context)
    {
        _context = context;
    }

    public async Task<TradieProfile?> GetByIdAsync(Guid id)
    {
        return await _context.TradieProfiles
            .Include(t => t.Services)
            .Include(t => t.Portfolio)
            .Include(t => t.Availability)
            .Include(t => t.ServiceLocations)
            .Include(t => t.Documents)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TradieProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _context.TradieProfiles
            .Include(t => t.Services)
            .Include(t => t.Portfolio)
            .Include(t => t.Availability)
            .Include(t => t.ServiceLocations)
            .FirstOrDefaultAsync(t => t.UserId == userId);
    }

    public async Task<List<TradieProfileSummaryDto>> SearchTradiesAsync(SearchTradiesRequest request)
    {
        var query = _context.TradieProfiles
            .Include(t => t.Services)
            .Include(t => t.ServiceLocations)
            .Where(t => t.VerificationStatus == VerificationStatus.Verified);

        // Filter by categories
        if (request.Categories?.Any() == true)
        {
            query = query.Where(t => t.Services.Any(s => request.Categories.Contains(s.Category)));
        }

        // Filter by rating
        if (request.MinRating.HasValue)
        {
            query = query.Where(t => t.Rating >= (double)request.MinRating.Value);
            // or: query = query.Where(t => (decimal)t.Rating >= request.MinRating.Value);
        }

        // Filter by availability
        if (request.AvailableNow == true)
        {
            query = query.Where(t => t.IsAvailable &&
                (t.AvailableFrom == null || t.AvailableFrom <= DateTime.UtcNow));
        }

        // Filter by emergency services
        if (request.EmergencyOnly == true)
        {
            query = query.Where(t => t.IsEmergencyService);
        }

        var tradies = await query
            .Select(t => new TradieProfileSummaryDto(
                t.Id,
                t.UserId,
                t.BusinessName,
                t.Description,
                t.Services.Select(s => s.Category).Distinct().ToList(),
                t.Rating,
                t.ReviewCount,
                t.IsAvailable,
                t.AvailableFrom,
                t.VerificationStatus,
                CalculateDistance(request.Latitude, request.Longitude,
                    t.ServiceLocations.First().Latitude, t.ServiceLocations.First().Longitude),
                t.IsEmergencyService,
                t.HourlyRate
            ))
            .Where(t => t.DistanceKm <= request.RadiusKm)
            .OrderBy(t => t.DistanceKm)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return tradies;
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Haversine formula for calculating distance between two points
        const double R = 6371; // Earth's radius in kilometers

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    private static double ToRadians(double degrees) => degrees * (Math.PI / 180);

    public async Task<TradieProfile> CreateAsync(TradieProfile profile)
    {
        _context.TradieProfiles.Add(profile);
        await _context.SaveChangesAsync();
        return profile;
    }

    public async Task<TradieProfile> UpdateAsync(TradieProfile profile)
    {
        _context.TradieProfiles.Update(profile);
        await _context.SaveChangesAsync();
        return profile;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.TradieProfiles.AnyAsync(t => t.Id == id);
    }

    public async Task<bool> IsABNUniqueAsync(string abn, Guid? excludeId = null)
    {
        var query = _context.TradieProfiles.Where(t => t.ABN == abn);

        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<List<TradieProfile>> GetAvailableTradiesAsync(double latitude, double longitude, double radiusKm)
    {
        return await _context.TradieProfiles
            .Include(t => t.Services)
            .Include(t => t.ServiceLocations)
            .Where(t => t.IsAvailable &&
                       t.VerificationStatus == VerificationStatus.Verified &&
                       (t.AvailableFrom == null || t.AvailableFrom <= DateTime.UtcNow))
            .ToListAsync();
    }

}