// ===== ENTITIES =====
// Entities/Job.cs
using FindTradie.Shared.Domain.Entities;
using FindTradie.Shared.Domain.Enums;
using System;
using System.Collections.Generic;

namespace FindTradie.Services.JobManagement.Entities;

public class Job : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ServiceCategory Category { get; set; }
    public string SubCategory { get; set; } = string.Empty;
    public JobUrgency Urgency { get; set; } = JobUrgency.Normal;
    public JobStatus Status { get; set; } = JobStatus.Posted;
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public DateTime? PreferredStartDate { get; set; }
    public DateTime? PreferredEndDate { get; set; }
    public bool IsFlexibleTiming { get; set; } = true;

    // Customer Information
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;

    // Location Information
    public string Address { get; set; } = string.Empty;
    public string Suburb { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostCode { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsLocationVisible { get; set; } = true;

    // Job Details
    public string? SpecialRequirements { get; set; }
    public bool RequiresLicense { get; set; } = false;
    public bool RequiresInsurance { get; set; } = true;
    public bool RequiresBackgroundCheck { get; set; } = false;

    // Completion Information
    public Guid? AcceptedQuoteId { get; set; }
    public Guid? AssignedTradieId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletionNotes { get; set; }
    public decimal? FinalAmount { get; set; }

    // Navigation Properties
    public List<JobImage> Images { get; set; } = new();
    public List<Quote> Quotes { get; set; } = new();
    public List<JobMessage> Messages { get; set; } = new();
    public List<JobStatusHistory> StatusHistory { get; set; } = new();
    public Quote? AcceptedQuote { get; set; }
}

public class Quote : BaseEntity
{
    public Guid JobId { get; set; }
    public Guid TradieId { get; set; }
    public string TradieBusinessName { get; set; } = string.Empty;
    public QuoteStatus Status { get; set; } = QuoteStatus.Pending;

    // Pricing Information
    public decimal MaterialsCost { get; set; }
    public decimal LabourCost { get; set; }
    public decimal TotalCost { get; set; }
    public string? PricingBreakdown { get; set; }

    // Timeline
    public int EstimatedDurationHours { get; set; }
    public DateTime? ProposedStartDate { get; set; }
    public DateTime? ProposedEndDate { get; set; }

    // Details
    public string Description { get; set; } = string.Empty;
    public string? MaterialsIncluded { get; set; }
    public string? Methodology { get; set; }
    public string? WarrantyOffered { get; set; }
    public DateTime ExpiresAt { get; set; }

    // Customer Response
    public DateTime? CustomerViewedAt { get; set; }
    public DateTime? CustomerRespondedAt { get; set; }
    public string? CustomerNotes { get; set; }
    public string? RejectionReason { get; set; }

    // Navigation Properties
    public Job Job { get; set; } = null!;
    public List<QuoteItem> Items { get; set; } = new();
}

public class QuoteItem : BaseEntity
{
    public Guid QuoteId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Quote Quote { get; set; } = null!;
}

public class JobImage : BaseEntity
{
    public Guid JobId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public ImageType ImageType { get; set; } = ImageType.Problem;
    public bool IsMainImage { get; set; } = false;
    public int DisplayOrder { get; set; }

    // Navigation
    public Job Job { get; set; } = null!;
}

public class JobMessage : BaseEntity
{
    public Guid JobId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public MessageType SenderType { get; set; }
    public string Message { get; set; } = string.Empty;
    public MessageType MessageType { get; set; } = MessageType.General;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public string? AttachmentUrl { get; set; }

    // Navigation
    public Job Job { get; set; } = null!;
}

public class JobStatusHistory : BaseEntity
{
    public Guid JobId { get; set; }
    public JobStatus FromStatus { get; set; }
    public JobStatus ToStatus { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public Guid ChangedBy { get; set; }
    public string ChangedByName { get; set; } = string.Empty;

    // Navigation
    public Job Job { get; set; } = null!;
}

public class JobPayment : BaseEntity
{
    public Guid JobId { get; set; }
    public Guid? QuoteId { get; set; }
    public string PaymentIntentId { get; set; } = string.Empty; // Stripe Payment Intent ID
    public decimal Amount { get; set; }
    public decimal PlatformFee { get; set; }
    public decimal TradieAmount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; }
    public string? PaymentMethodId { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
    public string? FailureReason { get; set; }
    public string? ReceiptUrl { get; set; }

    // Navigation
    public Job Job { get; set; } = null!;
    public Quote? Quote { get; set; }
}

// ===== ENUMS =====
public enum QuoteStatus
{
    Pending = 1,
    Submitted = 2,
    Viewed = 3,
    Accepted = 4,
    Rejected = 5,
    Expired = 6,
    Withdrawn = 7
}

public enum ImageType
{
    Problem = 1,
    Reference = 2,
    Progress = 3,
    Completed = 4,
    Before = 5,
    After = 6
}

public enum MessageType
{
    General = 1,
    QuoteSubmitted = 2,
    QuoteAccepted = 3,
    QuoteRejected = 4,
    JobStarted = 5,
    JobCompleted = 6,
    PaymentMade = 7,
    System = 8
}

public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Succeeded = 3,
    Failed = 4,
    Cancelled = 5,
    Refunded = 6,
    PartiallyRefunded = 7,
    Disputed = 8,
    Released = 9
}

public enum PaymentMethod
{
    CreditCard = 1,
    DebitCard = 2,
    PayPal = 3,
    BankTransfer = 4,
    ApplePay = 5,
    GooglePay = 6
}