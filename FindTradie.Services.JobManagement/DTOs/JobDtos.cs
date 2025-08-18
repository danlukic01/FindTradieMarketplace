// ===== DTOs =====
// DTOs/JobDtos.cs
using FindTradie.Services.JobManagement.Entities;
using FindTradie.Shared.Domain.Enums;
using System;
using System.Collections.Generic;

namespace FindTradie.Services.JobManagement.DTOs;

public record CreateJobRequest(
    string Title,
    string Description,
    ServiceCategory Category,
    string SubCategory,
    JobUrgency Urgency,
    decimal? BudgetMin,
    decimal? BudgetMax,
    DateTime? PreferredStartDate,
    DateTime? PreferredEndDate,
    bool IsFlexibleTiming,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string Address,
    string Suburb,
    string State,
    string PostCode,
    double Latitude,
    double Longitude,
    string? SpecialRequirements,
    bool RequiresLicense,
    bool RequiresInsurance,
    List<string>? ImageUrls = null
);

public record UpdateJobRequest(
    string Title,
    string Description,
    decimal? BudgetMin,
    decimal? BudgetMax,
    DateTime? PreferredStartDate,
    DateTime? PreferredEndDate,
    bool IsFlexibleTiming,
    string? SpecialRequirements
);

public record JobSummaryDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public ServiceCategory Category { get; init; }
    public string SubCategory { get; init; } = string.Empty;
    public JobUrgency Urgency { get; init; }
    public JobStatus Status { get; init; }
    public decimal? BudgetMin { get; init; }
    public decimal? BudgetMax { get; init; }
    public string Suburb { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public double DistanceKm { get; init; }
    public DateTime CreatedAt { get; init; }
    public int QuoteCount { get; init; }
    public bool HasImages { get; init; }
    public DateTime? PreferredStartDate { get; init; }
}

public record JobDetailDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public ServiceCategory Category { get; init; }
    public string SubCategory { get; init; } = string.Empty;
    public JobUrgency Urgency { get; init; }
    public JobStatus Status { get; init; }
    public decimal? BudgetMin { get; init; }
    public decimal? BudgetMax { get; init; }
    public DateTime? PreferredStartDate { get; init; }
    public DateTime? PreferredEndDate { get; init; }
    public bool IsFlexibleTiming { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public string CustomerPhone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string Suburb { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string PostCode { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? SpecialRequirements { get; init; }
    public bool RequiresLicense { get; init; }
    public bool RequiresInsurance { get; init; }
    public bool RequiresBackgroundCheck { get; init; }
    public Guid? AssignedTradieId { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public decimal? FinalAmount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<JobImageDto> Images { get; init; } = new();
    public List<QuoteSummaryDto> Quotes { get; init; } = new();
    public List<JobMessageDto> RecentMessages { get; init; } = new();
}

public record JobImageDto(
    Guid Id,
    string ImageUrl,
    string? Caption,
    string? Description,
    ImageType ImageType,
    bool IsMainImage,
    int DisplayOrder
);

public record QuoteSummaryDto(
    Guid Id,
    Guid TradieId,
    string TradieBusinessName,
    QuoteStatus Status,
    decimal TotalCost,
    int EstimatedDurationHours,
    DateTime? ProposedStartDate,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    bool IsExpired
);

public record QuoteDetailDto(
    Guid Id,
    Guid JobId,
    Guid TradieId,
    string TradieBusinessName,
    QuoteStatus Status,
    decimal MaterialsCost,
    decimal LabourCost,
    decimal TotalCost,
    string? PricingBreakdown,
    int EstimatedDurationHours,
    DateTime? ProposedStartDate,
    DateTime? ProposedEndDate,
    string Description,
    string? MaterialsIncluded,
    string? Methodology,
    string? WarrantyOffered,
    DateTime ExpiresAt,
    DateTime CreatedAt,
    List<QuoteItemDto> Items
);

public record QuoteItemDto(
    Guid Id,
    string Description,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    string? Notes
);

public record CreateQuoteRequest(
    Guid JobId,
    Guid TradieId,
    decimal MaterialsCost,
    decimal LabourCost,
    int EstimatedDurationHours,
    DateTime? ProposedStartDate,
    DateTime? ProposedEndDate,
    string Description,
    string? MaterialsIncluded,
    string? Methodology,
    string? WarrantyOffered,
    List<CreateQuoteItemRequest> Items
);

public record CreateQuoteItemRequest(
    string Description,
    int Quantity,
    decimal UnitPrice,
    string? Notes
);

public record UpdateQuoteStatusRequest(
    QuoteStatus Status,
    string? Notes
);

public record JobSearchRequest(
    double? Latitude = null,
    double? Longitude = null,
    double? RadiusKm = null,
    List<ServiceCategory>? Categories = null,
    JobUrgency? Urgency = null,
    decimal? MinBudget = null,
    decimal? MaxBudget = null,
    DateTime? StartDateFrom = null,
    DateTime? StartDateTo = null,
    bool? HasBudget = null,
    int PageNumber = 1,
    int PageSize = 20
);

public record JobMessageDto(
    Guid Id,
    Guid SenderId,
    string SenderName,
    MessageType SenderType,
    string Message,
    MessageType MessageType,
    bool IsRead,
    DateTime CreatedAt,
    string? AttachmentUrl
);

public record CreateJobMessageRequest(
    string Message,
    MessageType MessageType = MessageType.General,
    string? AttachmentUrl = null
);
