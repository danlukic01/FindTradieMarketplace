// Entities/TradieProfile.cs
using FindTradie.Shared.Domain.Entities;
using FindTradie.Shared.Domain.Enums;

namespace FindTradie.Services.TradieManagement.Entities;

public class TradieProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string ABN { get; set; } = string.Empty;
    public string? ACN { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public double ServiceRadius { get; set; } // in kilometers
    public bool IsAvailable { get; set; } = true;
    public DateTime? AvailableFrom { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public double Rating { get; set; } = 0.0;
    public int ReviewCount { get; set; } = 0;
    public bool IsEmergencyService { get; set; } = false;
    public string? InsuranceProvider { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public DateTime? InsuranceExpiryDate { get; set; }

    // Navigation properties
    public List<TradieService> Services { get; set; } = new();
    public List<TradieDocument> Documents { get; set; } = new();
    public List<PortfolioItem> Portfolio { get; set; } = new();
    public List<TradieAvailability> Availability { get; set; } = new();
    public List<TradieLocation> ServiceLocations { get; set; } = new();
}

public class TradieService : BaseEntity
{
    public Guid TradieProfileId { get; set; }
    public ServiceCategory Category { get; set; }
    public string SubCategory { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public TradieProfile TradieProfile { get; set; } = null!;
}

public class TradieDocument : BaseEntity
{
    public Guid TradieProfileId { get; set; }
    public DocumentType DocumentType { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public string? VerificationNotes { get; set; }
    public DateTime? VerificationDate { get; set; }
    public string? VerifiedBy { get; set; }

    // Navigation
    public TradieProfile TradieProfile { get; set; } = null!;
}

public class PortfolioItem : BaseEntity
{
    public Guid TradieProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? BeforeImageUrl { get; set; }
    public string? AfterImageUrl { get; set; }
    public ServiceCategory Category { get; set; }
    public decimal? ProjectValue { get; set; }
    public DateTime CompletionDate { get; set; }
    public bool IsPublic { get; set; } = true;

    // Navigation
    public TradieProfile TradieProfile { get; set; } = null!;
}

public class TradieAvailability : BaseEntity
{
    public Guid TradieProfileId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsEmergencyAvailable { get; set; } = false;

    // Navigation
    public TradieProfile TradieProfile { get; set; } = null!;
}

public class TradieLocation : BaseEntity
{
    public Guid TradieProfileId { get; set; }
    public string Suburb { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostCode { get; set; } = string.Empty;
    public double Latitude { get; set; }  // e.g., decimal(9,6)–(11,8) depending on your needs
    public double Longitude { get; set; }
    public bool IsPrimaryLocation { get; set; } = false;

    // Navigation
    public TradieProfile TradieProfile { get; set; } = null!;
}

public enum DocumentType
{
    License = 1,
    Insurance = 2,
    Certification = 3,
    BackgroundCheck = 4,
    BusinessRegistration = 5,
    SafetyTraining = 6,
    References = 7
}