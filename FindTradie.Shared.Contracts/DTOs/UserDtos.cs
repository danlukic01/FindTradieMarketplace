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

public record LoginRequest(
    string Email,
    string Password
);

public record UserProfileDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public UserType UserType { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsVerified { get; init; }
}

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