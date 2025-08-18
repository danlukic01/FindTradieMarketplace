// Services/JobApiService.cs
using FindTradie.Services.JobManagement.DTOs;
using FindTradie.Shared.Contracts.Common;
using FindTradie.Shared.Domain.Enums;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;

namespace FindTradie.Web.Services;

public class JobApiService : IJobApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly ILogger<JobApiService> _logger;

    public JobApiService(IHttpClientFactory httpClientFactory, IAuthService authService, ILogger<JobApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("FindTradieAPI");
        _authService = authService;
        _logger = logger;
    }

    private async Task SetAuthorizationHeaderAsync()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Request failed with status {StatusCode}: {Content}",
                response.StatusCode,
                responseContent);

            return new ApiResponse<T>
            {
                Success = false,
                Message = $"Request failed with status code {response.StatusCode}",
                Errors = string.IsNullOrWhiteSpace(responseContent)
                    ? null
                    : new List<string> { responseContent }
            };
        }

        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "Response was empty"
            };
        }

        return JsonSerializer.Deserialize<ApiResponse<T>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new ApiResponse<T> { Success = false, Message = "Failed to deserialize response" };
    }

    public async Task<ApiResponse<JobDetailDto>> CreateJobAsync(CreateJobRequest request)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/jobs", content);

            return await HandleResponse<JobDetailDto>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job");
            return new ApiResponse<JobDetailDto>
            {
                Success = false,
                Message = "An error occurred while creating the job",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<JobDetailDto>> GetJobAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/jobs/{id}");

            return await HandleResponse<JobDetailDto>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting job {Id}", id);
            return new ApiResponse<JobDetailDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the job",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<JobDetailDto>> UpdateJobAsync(Guid id, UpdateJobRequest request)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/jobs/{id}", content);

            return await HandleResponse<JobDetailDto>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job {Id}", id);
            return new ApiResponse<JobDetailDto>
            {
                Success = false,
                Message = "An error occurred while updating the job",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<List<JobSummaryDto>>> SearchJobsAsync(JobSearchRequest request)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/jobs/search", content);

            return await HandleResponse<List<JobSummaryDto>>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching jobs");
            return new ApiResponse<List<JobSummaryDto>>
            {
                Success = false,
                Message = "An error occurred while searching jobs",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<List<JobSummaryDto>>> GetCustomerJobsAsync(int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"/api/jobs/customer?pageNumber={pageNumber}&pageSize={pageSize}");

            return await HandleResponse<List<JobSummaryDto>>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer jobs");
            return new ApiResponse<List<JobSummaryDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving jobs",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<List<JobSummaryDto>>> GetTradieJobsAsync(int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"/api/jobs/tradie?pageNumber={pageNumber}&pageSize={pageSize}");

            return await HandleResponse<List<JobSummaryDto>>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tradie jobs");
            return new ApiResponse<List<JobSummaryDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving jobs",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> UpdateJobStatusAsync(Guid id, JobStatus status, string? reason = null)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var request = new { Status = status, Reason = reason };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"/api/jobs/{id}/status", content);

            return await HandleResponse<bool>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job status {Id}", id);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while updating job status",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> AssignTradieAsync(Guid jobId, Guid tradieId, Guid quoteId)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var request = new { TradieId = tradieId, QuoteId = quoteId };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/api/jobs/{jobId}/assign", content);

            return await HandleResponse<bool>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning tradie to job {JobId}", jobId);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while assigning tradie",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> CompleteJobAsync(Guid id, string? completionNotes = null)
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            var request = new { CompletionNotes = completionNotes };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/api/jobs/{id}/complete", content);

            return await HandleResponse<bool>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing job {Id}", id);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while completing job",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}