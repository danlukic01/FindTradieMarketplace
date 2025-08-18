// ===== CONTROLLERS =====
// Controllers/JobsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FindTradie.Services.JobManagement.Services;
using FindTradie.Services.JobManagement.DTOs;
using FindTradie.Shared.Contracts.Common;
using FindTradie.Shared.Domain.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace FindTradie.Services.JobManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    /// <summary>
    /// Create a new job posting
    /// </summary>
    [HttpPost]
    [Authorize(Roles = nameof(UserType.Customer))]
    public async Task<ActionResult<ApiResponse<JobDetailDto>>> CreateJob([FromBody] CreateJobRequest request)
    {
        var result = await _jobService.CreateJobAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get job details by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<JobDetailDto>>> GetJob(Guid id)
    {
        var result = await _jobService.GetJobAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Update job details
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = nameof(UserType.Customer))]
    public async Task<ActionResult<ApiResponse<JobDetailDto>>> UpdateJob(Guid id, [FromBody] UpdateJobRequest request)
    {
        var result = await _jobService.UpdateJobAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Search for jobs with filters
    /// </summary>
    [HttpPost("search")]
    [Authorize(Roles = nameof(UserType.Tradie))]
    public async Task<ActionResult<ApiResponse<List<JobSummaryDto>>>> SearchJobs([FromBody] JobSearchRequest request)
    {
        var result = await _jobService.SearchJobsAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Get jobs for the currently authenticated customer
    /// </summary>
    [HttpGet("customer")]
    [Authorize(Roles = nameof(UserType.Customer))]
    public async Task<ActionResult<ApiResponse<List<JobSummaryDto>>>> GetCustomerJobs(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var customerIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(customerIdClaim, out var customerId))
        {
            return Unauthorized(ApiResponse<List<JobSummaryDto>>.ErrorResult("Invalid user identifier"));
        }

        var result = await _jobService.GetCustomerJobsAsync(customerId, pageNumber, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get jobs for the currently authenticated tradie
    /// </summary>
    [HttpGet("tradie")]
    [Authorize(Roles = nameof(UserType.Tradie))]
    public async Task<ActionResult<ApiResponse<List<JobSummaryDto>>>> GetTradieJobs(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var tradieIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(tradieIdClaim, out var tradieId))
        {
            return Unauthorized(ApiResponse<List<JobSummaryDto>>.ErrorResult("Invalid user identifier"));
        }

        var result = await _jobService.GetTradieJobsAsync(tradieId, pageNumber, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Update job status
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = nameof(UserType.Customer))]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateJobStatus(
        Guid id,
        [FromBody] UpdateJobStatusRequest request)
    {
        var result = await _jobService.UpdateJobStatusAsync(id, request.Status, request.Reason);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Assign tradie to job (accept quote)
    /// </summary>
    [HttpPost("{id}/assign")]
    [Authorize(Roles = nameof(UserType.Customer))]
    public async Task<ActionResult<ApiResponse<bool>>> AssignTradie(
        Guid id,
        [FromBody] AssignTradieRequest request)
    {
        var result = await _jobService.AssignTradieAsync(id, request.TradieId, request.QuoteId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Mark job as completed
    /// </summary>
    [HttpPost("{id}/complete")]
    [Authorize(Roles = nameof(UserType.Customer))]
    public async Task<ActionResult<ApiResponse<bool>>> CompleteJob(
        Guid id,
        [FromBody] CompleteJobRequest request)
    {
        var result = await _jobService.CompleteJobAsync(id, request.CompletionNotes);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Add message to job
    /// </summary>
    [HttpPost("{id}/messages")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> AddJobMessage(
        Guid id,
        [FromBody] CreateJobMessageRequest request)
    {
        // TODO: Get sender ID from JWT token
        var senderId = Guid.Empty;
        var result = await _jobService.AddJobMessageAsync(id, senderId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}