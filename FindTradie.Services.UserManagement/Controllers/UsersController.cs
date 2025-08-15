// Controllers/UsersController.cs
using FindTradie.Shared.Contracts.Common;
using FindTradie.Shared.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FindTradie.Services.UserManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> Register(
        [FromBody] CreateUserRequest request)
    {
        try
        {
            // TODO: Implement user registration logic
            var userDto = new UserProfileDto(
                Guid.NewGuid(),
                request.Email,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.UserType,
                DateTime.UtcNow,
                false
            );

            return Ok(ApiResponse<UserProfileDto>.SuccessResult(
                userDto, "User registered successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return BadRequest(ApiResponse<UserProfileDto>.ErrorResult(
                "Registration failed"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetUser(Guid id)
    {
        try
        {
            // TODO: Implement get user logic
            return Ok(ApiResponse<UserProfileDto>.SuccessResult(
                null!, "User retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return NotFound(ApiResponse<UserProfileDto>.ErrorResult(
                "User not found"));
        }
    }
}