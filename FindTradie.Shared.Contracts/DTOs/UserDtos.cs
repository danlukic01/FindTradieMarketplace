using FindTradie.Shared.Domain.Enums;

namespace FindTradie.Shared.Contracts.DTOs;

public record CreateUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber,
    UserType UserType
);

public record UserProfileDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    UserType UserType,
    DateTime CreatedAt,
    bool IsVerified
);

public record TradieProfileDto(
    Guid Id,
    string BusinessName,
    string ABN,
    List<ServiceCategory> ServiceCategories,
    string Description,
    decimal HourlyRate,
    double ServiceRadius,
    bool IsAvailable,
    VerificationStatus VerificationStatus,
    double Rating,
    int ReviewCount
);

public record JobDto(
    Guid Id,
    string Title,
    string Description,
    ServiceCategory Category,
    JobUrgency Urgency,
    decimal? Budget,
    string Location,
    double Latitude,
    double Longitude,
    JobStatus Status,
    DateTime CreatedAt,
    Guid CustomerId
);