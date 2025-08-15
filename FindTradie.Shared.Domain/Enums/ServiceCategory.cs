// Enums/ServiceCategory.cs
namespace FindTradie.Shared.Domain.Enums;

public enum ServiceCategory
{
    Plumbing = 1,
    Electrical = 2,
    Carpentry = 3,
    Painting = 4,
    Roofing = 5,
    HVAC = 6,
    Landscaping = 7,
    Cleaning = 8,
    Handyman = 9,
    Emergency = 10
}

public enum JobStatus
{
    Posted = 1,
    QuoteRequested = 2,
    QuoteReceived = 3,
    Booked = 4,
    InProgress = 5,
    Completed = 6,
    Cancelled = 7,
    Disputed = 8
}

public enum JobUrgency
{
    Normal = 1,
    SameDay = 2,
    Emergency = 3
}

public enum UserType
{
    Customer = 1,
    Tradie = 2,
    Admin = 3
}

public enum VerificationStatus
{
    Pending = 1,
    InProgress = 2,
    Verified = 3,
    Rejected = 4,
    Expired = 5
}