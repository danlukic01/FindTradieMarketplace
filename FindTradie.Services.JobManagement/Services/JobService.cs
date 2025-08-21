// Services/JobService.cs
using AutoMapper;
using FindTradie.Services.JobManagement.Entities;
using FindTradie.Services.JobManagement.DTOs;
using FindTradie.Services.JobManagement.Repositories;
using FindTradie.Shared.Contracts.Common;
using FindTradie.Shared.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FindTradie.Services.JobManagement.Services;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly IQuoteRepository _quoteRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<JobService> _logger;

    public JobService(
        IJobRepository jobRepository,
        IQuoteRepository quoteRepository,
        IMapper mapper,
        ILogger<JobService> logger)
    {
        _jobRepository = jobRepository;
        _quoteRepository = quoteRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<JobDetailDto>> CreateJobAsync(CreateJobRequest request)
    {
        try
        {
            // Check if customer has too many active jobs
            var job = new Job
            {
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                SubCategory = request.SubCategory,
                Urgency = request.Urgency,
                BudgetMin = request.BudgetMin,
                BudgetMax = request.BudgetMax,
                PreferredStartDate = request.PreferredStartDate,
                PreferredEndDate = request.PreferredEndDate,
                IsFlexibleTiming = request.IsFlexibleTiming,
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                CustomerPhone = request.CustomerPhone,
                Address = request.Address,
                Suburb = request.Suburb,
                State = request.State,
                PostCode = request.PostCode,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                SpecialRequirements = request.SpecialRequirements,
                RequiresLicense = request.RequiresLicense,
                RequiresInsurance = request.RequiresInsurance,
                Status = JobStatus.Posted
            };

            // Add images if provided
            if (request.ImageUrls?.Any() == true)
            {
                for (int i = 0; i < request.ImageUrls.Count; i++)
                {
                    job.Images.Add(new JobImage
                    {
                        ImageUrl = request.ImageUrls[i],
                        ImageType = ImageType.Problem,
                        IsMainImage = i == 0,
                        DisplayOrder = i + 1
                    });
                }
            }

            // Add status history
            job.StatusHistory.Add(new JobStatusHistory
            {
                FromStatus = JobStatus.Posted,
                ToStatus = JobStatus.Posted,
                Reason = "Job created",
                ChangedBy = request.CustomerId,
                ChangedByName = "Customer"
            });

            var createdJob = await _jobRepository.CreateAsync(job);
            var jobDetail = await GetJobWithDetailsAsync(createdJob.Id);

            _logger.LogInformation("Created job {JobId} for customer {CustomerId}", createdJob.Id, request.CustomerId);

            return ApiResponse<JobDetailDto>.SuccessResult(jobDetail, "Job created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job for customer {CustomerId}", request.CustomerId);
            return ApiResponse<JobDetailDto>.ErrorResult("Failed to create job");
        }
    }

    public async Task<ApiResponse<JobDetailDto>> GetJobAsync(Guid id)
    {
        try
        {
            var jobDetail = await GetJobWithDetailsAsync(id);
            if (jobDetail == null)
            {
                return ApiResponse<JobDetailDto>.ErrorResult("Job not found");
            }

            return ApiResponse<JobDetailDto>.SuccessResult(jobDetail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job {JobId}", id);
            return ApiResponse<JobDetailDto>.ErrorResult("Failed to retrieve job");
        }
    }

    public async Task<ApiResponse<JobDetailDto>> UpdateJobAsync(Guid id, UpdateJobRequest request)
    {
        try
        {
            var job = await _jobRepository.UpdateTrackedAsync(id, job =>
            {
                job.Title = request.Title ?? job.Title;
                job.Description = request.Description ?? job.Description;

                if (request.Category.HasValue)
                    job.Category = request.Category.Value;
                if (request.Urgency.HasValue)
                    job.Urgency = request.Urgency.Value;

                job.Suburb = request.Suburb ?? job.Suburb;
                job.PostCode = request.PostCode ?? job.PostCode;
                job.Address = request.Address ?? job.Address;
                job.BudgetMin = request.BudgetMin ?? job.BudgetMin;
                job.BudgetMax = request.BudgetMax ?? job.BudgetMax;
                job.PreferredStartDate = request.PreferredStartDate ?? job.PreferredStartDate;
                job.PreferredEndDate = request.PreferredEndDate ?? job.PreferredEndDate;

                if (request.IsFlexibleTiming.HasValue)
                    job.IsFlexibleTiming = request.IsFlexibleTiming.Value;

                job.SpecialRequirements = request.SpecialRequirements ?? job.SpecialRequirements;

                if (request.RemovedImageIds?.Any() == true)
                {
                    var imagesToRemove = job.Images
                        .Where(img => request.RemovedImageIds.Contains(img.Id))
                        .ToList();

                    foreach (var image in imagesToRemove)
                    {
                        image.IsDeleted = true;
                    }
                }

                if (request.ImageUrls?.Any() == true)
                {
                    var maxOrder = job.Images.Any() ? job.Images.Max(i => i.DisplayOrder) : 0;

                    for (int i = 0; i < request.ImageUrls.Count; i++)
                    {
                        var newImage = new JobImage
                        {
                            Id = Guid.NewGuid(),
                            JobId = job.Id,
                            ImageUrl = request.ImageUrls[i],
                            ImageType = ImageType.Problem,
                            IsMainImage = !job.Images.Any(x => !x.IsDeleted && x.IsMainImage) && i == 0,
                            DisplayOrder = maxOrder + i + 1,
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        };

                        job.Images.Add(newImage);
                    }
                }
            });

            var jobDetail = await GetJobWithDetailsAsync(id);
            return ApiResponse<JobDetailDto>.SuccessResult(jobDetail, "Job updated successfully");
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<JobDetailDto>.ErrorResult(ex.Message);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency error updating job {JobId}", id);
            return ApiResponse<JobDetailDto>.ErrorResult("The job was modified by another user. Please reload and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job {JobId}", id);
            return ApiResponse<JobDetailDto>.ErrorResult("Failed to update job");
        }
    }

    public async Task<ApiResponse<List<JobSummaryDto>>> SearchJobsAsync(JobSearchRequest request)
    {
        try
        {
            var results = await _jobRepository.SearchJobsAsync(request);
            return ApiResponse<List<JobSummaryDto>>.SuccessResult(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching jobs");
            return ApiResponse<List<JobSummaryDto>>.ErrorResult("Failed to search jobs");
        }
    }

    public async Task<ApiResponse<List<JobSummaryDto>>> GetCustomerJobsAsync(Guid customerId, int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            var jobs = await _jobRepository.GetJobsByCustomerAsync(customerId, pageNumber, pageSize);
            var jobSummaries = _mapper.Map<List<JobSummaryDto>>(jobs);
            return ApiResponse<List<JobSummaryDto>>.SuccessResult(jobSummaries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving jobs for customer {CustomerId}", customerId);
            return ApiResponse<List<JobSummaryDto>>.ErrorResult("Failed to retrieve customer jobs");
        }
    }

    public async Task<ApiResponse<List<JobSummaryDto>>> GetTradieJobsAsync(Guid tradieId, int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            var jobs = await _jobRepository.GetJobsByTradieAsync(tradieId, pageNumber, pageSize);
            var jobSummaries = _mapper.Map<List<JobSummaryDto>>(jobs);
            return ApiResponse<List<JobSummaryDto>>.SuccessResult(jobSummaries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving jobs for tradie {TradieId}", tradieId);
            return ApiResponse<List<JobSummaryDto>>.ErrorResult("Failed to retrieve tradie jobs");
        }
    }

    public async Task<ApiResponse<bool>> UpdateJobStatusAsync(Guid id, JobStatus status, string? reason = null)
    {
        try
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return ApiResponse<bool>.ErrorResult("Job not found");
            }

            var oldStatus = job.Status;
            job.Status = status;

            // Add status history
            job.StatusHistory.Add(new JobStatusHistory
            {
                FromStatus = oldStatus,
                ToStatus = status,
                Reason = reason ?? $"Status changed to {status}",
                ChangedBy = Guid.Empty, // TODO: Get from context
                ChangedByName = "System"
            });

            await _jobRepository.UpdateAsync(job);

            _logger.LogInformation("Updated job {JobId} status from {OldStatus} to {NewStatus}",
                id, oldStatus, status);

            return ApiResponse<bool>.SuccessResult(true, "Job status updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job status for job {JobId}", id);
            return ApiResponse<bool>.ErrorResult("Failed to update job status");
        }
    }

    public async Task<ApiResponse<bool>> AssignTradieAsync(Guid jobId, Guid tradieId, Guid quoteId)
    {
        try
        {
            var job = await _jobRepository.GetByIdWithDetailsAsync(jobId);
            if (job == null)
            {
                return ApiResponse<bool>.ErrorResult("Job not found");
            }

            var quote = job.Quotes.FirstOrDefault(q => q.Id == quoteId);
            if (quote == null || quote.TradieId != tradieId)
            {
                return ApiResponse<bool>.ErrorResult("Quote not found or does not belong to the specified tradie");
            }

            if (quote.Status != QuoteStatus.Submitted)
            {
                return ApiResponse<bool>.ErrorResult("Quote is not in a valid status to be accepted");
            }

            // Update job
            job.AssignedTradieId = tradieId;
            job.AcceptedQuoteId = quoteId;
            job.Status = JobStatus.Booked;

            // Update quote
            quote.Status = QuoteStatus.Accepted;
            quote.CustomerRespondedAt = DateTime.UtcNow;

            // Reject other quotes
            foreach (var otherQuote in job.Quotes.Where(q => q.Id != quoteId && q.Status == QuoteStatus.Submitted))
            {
                otherQuote.Status = QuoteStatus.Rejected;
                otherQuote.RejectionReason = "Another quote was accepted";
            }

            // Add status history
            job.StatusHistory.Add(new JobStatusHistory
            {
                FromStatus = JobStatus.QuoteReceived,
                ToStatus = JobStatus.Booked,
                Reason = "Quote accepted and tradie assigned",
                ChangedBy = job.CustomerId,
                ChangedByName = "Customer"
            });

            await _jobRepository.UpdateAsync(job);

            _logger.LogInformation("Assigned tradie {TradieId} to job {JobId} with quote {QuoteId}",
                tradieId, jobId, quoteId);

            return ApiResponse<bool>.SuccessResult(true, "Tradie assigned successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning tradie {TradieId} to job {JobId}", tradieId, jobId);
            return ApiResponse<bool>.ErrorResult("Failed to assign tradie");
        }
    }

    public async Task<ApiResponse<bool>> CompleteJobAsync(Guid id, string? completionNotes = null)
    {
        try
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return ApiResponse<bool>.ErrorResult("Job not found");
            }

            if (job.Status != JobStatus.InProgress)
            {
                return ApiResponse<bool>.ErrorResult("Job must be in progress to be completed");
            }

            job.Status = JobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
            job.CompletionNotes = completionNotes;

            // Add status history
            job.StatusHistory.Add(new JobStatusHistory
            {
                FromStatus = JobStatus.InProgress,
                ToStatus = JobStatus.Completed,
                Reason = "Job completed",
                Notes = completionNotes,
                ChangedBy = job.AssignedTradieId ?? Guid.Empty,
                ChangedByName = "Tradie"
            });

            await _jobRepository.UpdateAsync(job);

            _logger.LogInformation("Completed job {JobId}", id);

            return ApiResponse<bool>.SuccessResult(true, "Job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing job {JobId}", id);
            return ApiResponse<bool>.ErrorResult("Failed to complete job");
        }
    }

    public async Task<ApiResponse<bool>> AddJobMessageAsync(Guid jobId, Guid senderId, CreateJobMessageRequest request)
    {
        try
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null)
            {
                return ApiResponse<bool>.ErrorResult("Job not found");
            }

            var message = new JobMessage
            {
                JobId = jobId,
                SenderId = senderId,
                SenderName = "User", // TODO: Get from user service
                SenderType = senderId == job.CustomerId ? MessageType.General : MessageType.General,
                Message = request.Message,
                MessageType = request.MessageType,
                AttachmentUrl = request.AttachmentUrl
            };

            job.Messages.Add(message);
            await _jobRepository.UpdateAsync(job);

            return ApiResponse<bool>.SuccessResult(true, "Message added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding message to job {JobId}", jobId);
            return ApiResponse<bool>.ErrorResult("Failed to add message");
        }
    }

    private async Task<JobDetailDto?> GetJobWithDetailsAsync(Guid id)
    {
        var job = await _jobRepository.GetByIdWithDetailsAsync(id);
        return job == null ? null : _mapper.Map<JobDetailDto>(job);
    }
}