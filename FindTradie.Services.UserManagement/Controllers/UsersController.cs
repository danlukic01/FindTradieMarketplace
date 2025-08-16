// Controllers/UsersController.cs
using FindTradie.Shared.Contracts.Common;
using FindTradie.Shared.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;
using FindTradie.Services.UserManagement.Services;

namespace FindTradie.Services.UserManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> Register(
        [FromBody] CreateUserRequest request)
    {
        try
        {
            var result = await _userService.CreateUserAsync(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return BadRequest(ApiResponse<UserProfileDto>.ErrorResult(
                "Registration failed"));
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<string>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _userService.LoginAsync(request.Email, request.Password);
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user {Email}", request.Email);
            return Unauthorized(ApiResponse<string>.ErrorResult("Login failed"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetUser(Guid id)
    {
        try
        {
            var result = await _userService.GetUserAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return NotFound(ApiResponse<UserProfileDto>.ErrorResult(
                "User not found"));
        }
    }
}