// Services/IJobApiService.cs
using FindTradie.Services.JobManagement.DTOs;
using FindTradie.Shared.Contracts.Common;
using FindTradie.Shared.Domain.Enums;

namespace FindTradie.Web.Services;

public interface IJobApiService
{
    Task<ApiResponse<JobDetailDto>> CreateJobAsync(CreateJobRequest request);
    Task<ApiResponse<JobDetailDto>> GetJobAsync(Guid id);
    Task<ApiResponse<JobDetailDto>> UpdateJobAsync(Guid id, UpdateJobRequest request);
    Task<ApiResponse<List<JobSummaryDto>>> SearchJobsAsync(JobSearchRequest request);
    Task<ApiResponse<List<JobSummaryDto>>> GetCustomerJobsAsync(int pageNumber = 1, int pageSize = 20);
    Task<ApiResponse<List<JobSummaryDto>>> GetTradieJobsAsync(int pageNumber = 1, int pageSize = 20);
    Task<ApiResponse<bool>> UpdateJobStatusAsync(Guid id, JobStatus status, string? reason = null);
    Task<ApiResponse<bool>> AssignTradieAsync(Guid jobId, Guid tradieId, Guid quoteId);
    Task<ApiResponse<bool>> CompleteJobAsync(Guid id, string? completionNotes = null);
}