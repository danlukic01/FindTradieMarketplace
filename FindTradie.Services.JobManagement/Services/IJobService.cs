// Services/IJobService.cs
using FindTradie.Services.JobManagement.DTOs;
using FindTradie.Shared.Contracts.Common;
using FindTradie.Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FindTradie.Services.JobManagement.Services;

public interface IJobService
{
    Task<ApiResponse<JobDetailDto>> CreateJobAsync(CreateJobRequest request);
    Task<ApiResponse<JobDetailDto>> GetJobAsync(Guid id);
    Task<ApiResponse<JobDetailDto>> UpdateJobAsync(Guid id, UpdateJobRequest request);
    Task<ApiResponse<List<JobSummaryDto>>> SearchJobsAsync(JobSearchRequest request);
    Task<ApiResponse<List<JobSummaryDto>>> GetCustomerJobsAsync(Guid customerId, int pageNumber = 1, int pageSize = 20);
    Task<ApiResponse<List<JobSummaryDto>>> GetTradieJobsAsync(Guid tradieId, int pageNumber = 1, int pageSize = 20);
    Task<ApiResponse<bool>> UpdateJobStatusAsync(Guid id, JobStatus status, string? reason = null);
    Task<ApiResponse<bool>> AssignTradieAsync(Guid jobId, Guid tradieId, Guid quoteId);
    Task<ApiResponse<bool>> CompleteJobAsync(Guid id, string? completionNotes = null);
    Task<ApiResponse<bool>> AddJobMessageAsync(Guid jobId, Guid senderId, CreateJobMessageRequest request);
    //Task GetTradieJobsAsync(Guid tradieId, int pageNumber, int pageSize);
}