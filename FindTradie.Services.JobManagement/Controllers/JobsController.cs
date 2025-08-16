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
using System.Security.Claims;

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
    [Authorize]
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
    [Authorize]
    public async Task<ActionResult<ApiResponse<JobDetailDto>>> UpdateJob(Guid id, [FromBody] UpdateJobRequest request)
    {
        var result = await _jobService.UpdateJobAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Search for jobs with filters
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse<List<JobSummaryDto>>>> SearchJobs([FromBody] JobSearchRequest request)
    {
        var result = await _jobService.SearchJobsAsync(request);
        return Ok(result);
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("my-jobs")]
    public async Task<IActionResult> GetMyJobs()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userId, out var customerId))
        {
            var result = await _jobService.GetCustomerJobsAsync(customerId);
            return Ok(result);
        }
        return Unauthorized();
    }

    [HttpPost("post")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> PostJob([FromBody] CreateJobRequest request)
    {
        var result = await _jobService.CreateJobAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = "Tradie")]
    [HttpGet("browse")]
    public async Task<IActionResult> BrowseJobs([FromQuery] JobSearchRequest filters)
    {
        var result = await _jobService.SearchJobsAsync(filters);
        return Ok(result);
    }

    /// <summary>
    /// Get jobs for a specific customer
    /// </summary>
    [HttpGet("customer/{customerId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<JobSummaryDto>>>> GetCustomerJobs(
        Guid customerId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _jobService.GetCustomerJobsAsync(customerId, pageNumber, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get jobs for a specific tradie
    /// </summary>
    [HttpGet("tradie/{tradieId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<JobSummaryDto>>>> GetTradieJobs(
        Guid tradieId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _jobService.GetTradieJobsAsync(tradieId, pageNumber, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Update job status
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize]
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
    [Authorize]
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
    [Authorize]
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