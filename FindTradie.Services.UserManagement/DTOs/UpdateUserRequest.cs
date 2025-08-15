// DTOs/UpdateUserRequest.cs
namespace FindTradie.Services.UserManagement.DTOs;

public record UpdateUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber
);