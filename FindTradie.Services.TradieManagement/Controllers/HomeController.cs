// ===== CONTROLLERS =====
// Controllers/TradiesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FindTradie.Services.TradieManagement.Services;
using FindTradie.Services.TradieManagement.DTOs;
using FindTradie.Shared.Domain.Enums;
using FindTradie.Shared.Contracts.Common;

namespace FindTradie.Services.TradieManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TradiesController : ControllerBase
{
    private readonly ITradieService _tradieService;

    public TradiesController(ITradieService tradieService)
    {
        _tradieService = tradieService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<TradieProfileDetailDto>>> CreateProfile(
        [FromBody] CreateTradieProfileRequest request)
    {
        var result = await _tradieService.CreateProfileAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TradieProfileDetailDto>>> GetProfile(Guid id)
    {
        var result = await _tradieService.GetProfileAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<TradieProfileDetailDto>>> GetProfileByUserId(Guid userId)
    {
        var result = await _tradieService.GetProfileByUserIdAsync(userId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<TradieProfileDetailDto>>> UpdateProfile(
        Guid id, [FromBody] UpdateTradieProfileRequest request)
    {
        var result = await _tradieService.UpdateProfileAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse<List<TradieProfileSummaryDto>>>> SearchTradies(
        [FromBody] SearchTradiesRequest request)
    {
        var result = await _tradieService.SearchTradiesAsync(request);
        return Ok(result);
    }

    [HttpPatch("{id}/availability")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateAvailability(
        Guid id, [FromBody] UpdateAvailabilityRequest request)
    {
        var result = await _tradieService.UpdateAvailabilityAsync(id, request.IsAvailable, request.AvailableFrom);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}/verify")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> VerifyTradie(
        Guid id, [FromBody] VerifyTradieRequest request)
    {
        var result = await _tradieService.VerifyTradieAsync(id, request.Status, request.Notes);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

public record UpdateAvailabilityRequest(bool IsAvailable, DateTime? AvailableFrom = null);
public record VerifyTradieRequest(VerificationStatus Status, string? Notes = null);