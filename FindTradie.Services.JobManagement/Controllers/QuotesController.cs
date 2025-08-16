// Controllers/QuotesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FindTradie.Services.JobManagement.Services;
using FindTradie.Services.JobManagement.DTOs;
using FindTradie.Shared.Contracts.Common;
using System.Threading.Tasks;
using System;
using FindTradie.Shared.Domain.Enums;
using System.Collections.Generic;

namespace FindTradie.Services.JobManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quoteService;

    public QuotesController(IQuoteService quoteService)
    {
        _quoteService = quoteService;
    }

    /// <summary>
    /// Submit a quote for a job
    /// </summary>
    [HttpPost]
    [Authorize(Roles = nameof(UserType.Tradie))]
    public async Task<ActionResult<ApiResponse<QuoteDetailDto>>> CreateQuote([FromBody] CreateQuoteRequest request)
    {
        var result = await _quoteService.CreateQuoteAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get quote details by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<QuoteDetailDto>>> GetQuote(Guid id)
    {
        var result = await _quoteService.GetQuoteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Get all quotes for a specific job
    /// </summary>
    [HttpGet("job/{jobId}")]
    [Authorize(Roles = nameof(UserType.Customer))]
    public async Task<ActionResult<ApiResponse<List<QuoteSummaryDto>>>> GetQuotesByJob(Guid jobId)
    {
        var result = await _quoteService.GetQuotesByJobAsync(jobId);
        return Ok(result);
    }

    /// <summary>
    /// Get quotes submitted by a specific tradie
    /// </summary>
    [HttpGet("tradie/{tradieId}")]
    [Authorize(Roles = nameof(UserType.Tradie))]
    public async Task<ActionResult<ApiResponse<List<QuoteSummaryDto>>>> GetQuotesByTradie(
        Guid tradieId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _quoteService.GetQuotesByTradieAsync(tradieId, pageNumber, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Update quote status (accept/reject)
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = nameof(UserType.Customer))]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateQuoteStatus(
        Guid id,
        [FromBody] UpdateQuoteStatusRequest request)
    {
        var result = await _quoteService.UpdateQuoteStatusAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Withdraw a submitted quote
    /// </summary>
    [HttpPost("{id}/withdraw")]
    [Authorize(Roles = nameof(UserType.Tradie))]
    public async Task<ActionResult<ApiResponse<bool>>> WithdrawQuote(
        Guid id,
        [FromBody] WithdrawQuoteRequest request)
    {
        var result = await _quoteService.WithdrawQuoteAsync(id, request.Reason);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
// ===== REQUEST/RESPONSE MODELS =====
public record UpdateJobStatusRequest(JobStatus Status, string? Reason = null);
public record AssignTradieRequest(Guid TradieId, Guid QuoteId);
public record CompleteJobRequest(string? CompletionNotes = null);
public record WithdrawQuoteRequest(string Reason);