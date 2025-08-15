// Services/QuoteService.cs
using AutoMapper;
using FindTradie.Services.JobManagement.Entities;
using FindTradie.Services.JobManagement.DTOs;
using FindTradie.Services.JobManagement.Repositories;
using FindTradie.Shared.Contracts.Common;
using FindTradie.Shared.Domain.Enums;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace FindTradie.Services.JobManagement.Services;

public class QuoteService : IQuoteService
{
    private readonly IQuoteRepository _quoteRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<QuoteService> _logger;

    public QuoteService(
        IQuoteRepository quoteRepository,
        IJobRepository jobRepository,
        IMapper mapper,
        ILogger<QuoteService> logger)
    {
        _quoteRepository = quoteRepository;
        _jobRepository = jobRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<QuoteDetailDto>> CreateQuoteAsync(CreateQuoteRequest request)
    {
        try
        {
            // Check if job exists and is accepting quotes
            var job = await _jobRepository.GetByIdAsync(request.JobId);
            if (job == null)
            {
                return ApiResponse<QuoteDetailDto>.ErrorResult("Job not found");
            }

            if (job.Status != JobStatus.Posted && job.Status != JobStatus.QuoteRequested)
            {
                return ApiResponse<QuoteDetailDto>.ErrorResult("Job is not accepting quotes");
            }

            // Check if tradie has already quoted for this job
            var hasQuoted = await _quoteRepository.HasTradieQuotedForJobAsync(request.TradieId, request.JobId);
            if (hasQuoted)
            {
                return ApiResponse<QuoteDetailDto>.ErrorResult("You have already submitted a quote for this job");
            }

            var quote = new Quote
            {
                JobId = request.JobId,
                TradieId = request.TradieId,
                TradieBusinessName = "Business Name", // TODO: Get from tradie service
                MaterialsCost = request.MaterialsCost,
                LabourCost = request.LabourCost,
                TotalCost = request.MaterialsCost + request.LabourCost,
                EstimatedDurationHours = request.EstimatedDurationHours,
                ProposedStartDate = request.ProposedStartDate,
                ProposedEndDate = request.ProposedEndDate,
                Description = request.Description,
                MaterialsIncluded = request.MaterialsIncluded,
                Methodology = request.Methodology,
                WarrantyOffered = request.WarrantyOffered,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // Quotes expire in 7 days
                Status = QuoteStatus.Submitted
            };

            // Add quote items
            foreach (var item in request.Items)
            {
                quote.Items.Add(new QuoteItem
                {
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.Quantity * item.UnitPrice,
                    Notes = item.Notes
                });
            }

            var createdQuote = await _quoteRepository.CreateAsync(quote);

            // Update job status if this is the first quote
            if (job.Status == JobStatus.Posted)
            {
                job.Status = JobStatus.QuoteReceived;
                await _jobRepository.UpdateAsync(job);
            }

            var quoteDetail = _mapper.Map<QuoteDetailDto>(createdQuote);

            _logger.LogInformation("Created quote {QuoteId} for job {JobId} by tradie {TradieId}",
                createdQuote.Id, request.JobId, request.TradieId);

            return ApiResponse<QuoteDetailDto>.SuccessResult(quoteDetail, "Quote submitted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quote for job {JobId}", request.JobId);
            return ApiResponse<QuoteDetailDto>.ErrorResult("Failed to create quote");
        }
    }

    public async Task<ApiResponse<QuoteDetailDto>> GetQuoteAsync(Guid id)
    {
        try
        {
            var quote = await _quoteRepository.GetByIdWithDetailsAsync(id);
            if (quote == null)
            {
                return ApiResponse<QuoteDetailDto>.ErrorResult("Quote not found");
            }

            var quoteDetail = _mapper.Map<QuoteDetailDto>(quote);
            return ApiResponse<QuoteDetailDto>.SuccessResult(quoteDetail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quote {QuoteId}", id);
            return ApiResponse<QuoteDetailDto>.ErrorResult("Failed to retrieve quote");
        }
    }

    public async Task<ApiResponse<List<QuoteSummaryDto>>> GetQuotesByJobAsync(Guid jobId)
    {
        try
        {
            var quotes = await _quoteRepository.GetQuotesByJobAsync(jobId);
            var quoteSummaries = _mapper.Map<List<QuoteSummaryDto>>(quotes);
            return ApiResponse<List<QuoteSummaryDto>>.SuccessResult(quoteSummaries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quotes for job {JobId}", jobId);
            return ApiResponse<List<QuoteSummaryDto>>.ErrorResult("Failed to retrieve quotes");
        }
    }

    public async Task<ApiResponse<List<QuoteSummaryDto>>> GetQuotesByTradieAsync(Guid tradieId, int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            var quotes = await _quoteRepository.GetQuotesByTradieAsync(tradieId, pageNumber, pageSize);
            var quoteSummaries = _mapper.Map<List<QuoteSummaryDto>>(quotes);
            return ApiResponse<List<QuoteSummaryDto>>.SuccessResult(quoteSummaries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quotes for tradie {TradieId}", tradieId);
            return ApiResponse<List<QuoteSummaryDto>>.ErrorResult("Failed to retrieve quotes");
        }
    }

    public async Task<ApiResponse<bool>> UpdateQuoteStatusAsync(Guid id, UpdateQuoteStatusRequest request)
    {
        try
        {
            var quote = await _quoteRepository.GetByIdAsync(id);
            if (quote == null)
            {
                return ApiResponse<bool>.ErrorResult("Quote not found");
            }

            quote.Status = request.Status;
            quote.CustomerRespondedAt = DateTime.UtcNow;
            quote.CustomerNotes = request.Notes;

            if (request.Status == QuoteStatus.Rejected)
            {
                quote.RejectionReason = request.Notes;
            }

            await _quoteRepository.UpdateAsync(quote);

            _logger.LogInformation("Updated quote {QuoteId} status to {Status}", id, request.Status);

            return ApiResponse<bool>.SuccessResult(true, "Quote status updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quote status for quote {QuoteId}", id);
            return ApiResponse<bool>.ErrorResult("Failed to update quote status");
        }
    }

    public async Task<ApiResponse<bool>> WithdrawQuoteAsync(Guid id, string reason)
    {
        try
        {
            var quote = await _quoteRepository.GetByIdAsync(id);
            if (quote == null)
            {
                return ApiResponse<bool>.ErrorResult("Quote not found");
            }

            if (quote.Status != QuoteStatus.Submitted)
            {
                return ApiResponse<bool>.ErrorResult("Only submitted quotes can be withdrawn");
            }

            quote.Status = QuoteStatus.Withdrawn;
            quote.RejectionReason = reason;

            await _quoteRepository.UpdateAsync(quote);

            _logger.LogInformation("Withdrew quote {QuoteId} with reason: {Reason}", id, reason);

            return ApiResponse<bool>.SuccessResult(true, "Quote withdrawn successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error withdrawing quote {QuoteId}", id);
            return ApiResponse<bool>.ErrorResult("Failed to withdraw quote");
        }
    }
}