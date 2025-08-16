using System;

namespace FindTradie.Shared.Contracts.DTOs;

public record QuoteDto(
    Guid Id,
    string TradieName,
    string TradieInitials,
    string JobTitle,
    decimal Amount,
    double Rating
);

public record TradieDto(
    Guid Id,
    string BusinessName,
    string Initials,
    string MainCategory,
    double Rating,
    int ReviewCount,
    bool IsAvailable
);

public record JobLeadDto(
    Guid Id,
    string Category,
    string Urgency,
    string Title,
    string Description,
    string Location,
    double Distance,
    string Budget,
    string TimeAgo
);

public record ActiveJobDto(
    Guid Id,
    string Title,
    string CustomerName,
    string Status,
    decimal Amount,
    DateTime DueDate,
    Guid CustomerId
);
