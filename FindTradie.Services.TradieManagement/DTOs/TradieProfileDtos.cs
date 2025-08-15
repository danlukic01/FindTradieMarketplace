// ===== DATA TRANSFER OBJECTS =====
// DTOs/TradieProfileDtos.cs
using FindTradie.Shared.Domain.Enums;

namespace FindTradie.Services.TradieManagement.DTOs;

public record CreateTradieProfileRequest(
    Guid UserId,
    string BusinessName,
    string ABN,
    string? ACN,
    string Description,
    decimal HourlyRate,
    double ServiceRadius,
    List<ServiceCategory> ServiceCategories,
    bool IsEmergencyService,
    string? InsuranceProvider,
    string? InsurancePolicyNumber,
    DateTime? InsuranceExpiryDate
);

public record UpdateTradieProfileRequest(
    string BusinessName,
    string Description,
    decimal HourlyRate,
    double ServiceRadius,
    bool IsEmergencyService,
    bool IsAvailable
);

public record TradieProfileSummaryDto(
    Guid Id,
    Guid UserId,
    string BusinessName,
    string Description,
    List<ServiceCategory> ServiceCategories,
    double Rating,
    int ReviewCount,
    bool IsAvailable,
    DateTime? AvailableFrom,
    VerificationStatus VerificationStatus,
    double DistanceKm,
    bool IsEmergencyService,
    decimal HourlyRate
);

public record TradieProfileDetailDto(
    Guid Id,
    Guid UserId,
    string BusinessName,
    string ABN,
    string Description,
    decimal HourlyRate,
    double ServiceRadius,
    bool IsAvailable,
    DateTime? AvailableFrom,
    VerificationStatus VerificationStatus,
    double Rating,
    int ReviewCount,
    bool IsEmergencyService,
    string? InsuranceProvider,
    DateTime? InsuranceExpiryDate,
    List<TradieServiceDto> Services,
    List<PortfolioItemDto> Portfolio,
    List<TradieAvailabilityDto> Availability,
    DateTime CreatedAt
);

public record TradieServiceDto(
    Guid Id,
    ServiceCategory Category,
    string SubCategory,
    string Description,
    decimal? MinPrice,
    decimal? MaxPrice
);

public record PortfolioItemDto(
    Guid Id,
    string Title,
    string Description,
    string ImageUrl,
    string? BeforeImageUrl,
    string? AfterImageUrl,
    ServiceCategory Category,
    decimal? ProjectValue,
    DateTime CompletionDate
);

public record TradieAvailabilityDto(
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    bool IsAvailable,
    bool IsEmergencyAvailable
);

public record SearchTradiesRequest(
    double Latitude,
    double Longitude,
    double RadiusKm,
    List<ServiceCategory>? Categories = null,
    decimal? MinRating = null,
    bool? AvailableNow = null,
    bool? EmergencyOnly = null,
    int PageNumber = 1,
    int PageSize = 20
);