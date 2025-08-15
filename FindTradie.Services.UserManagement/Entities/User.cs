// Entities/User.cs
using FindTradie.Shared.Domain.Entities;
using FindTradie.Shared.Domain.Enums;

namespace FindTradie.Services.UserManagement.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public bool IsEmailVerified { get; set; } = false;
    public bool IsPhoneVerified { get; set; } = false;
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public List<UserAddress> Addresses { get; set; } = new();
    public UserProfile? Profile { get; set; }
}

public class UserAddress : BaseEntity
{
    public Guid UserId { get; set; }
    public string Street { get; set; } = string.Empty;
    public string Suburb { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostCode { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsDefault { get; set; } = false;

    // Navigation
    public User User { get; set; } = null!;
}

public class UserProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Bio { get; set; }
    public string? PreferredContactMethod { get; set; }
    public bool IsNotificationsEnabled { get; set; } = true;

    // Navigation
    public User User { get; set; } = null!;
}