// Repositories/IJobRepository.cs
using FindTradie.Services.JobManagement.Entities;
using FindTradie.Services.JobManagement.DTOs;
using FindTradie.Services.JobManagement.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using FindTradie.Shared.Domain.Enums;
using System.Linq;

namespace FindTradie.Services.JobManagement.Repositories;

public interface IJobRepository
{
    Task<Job?> GetByIdAsync(Guid id);
    Task<Job?> GetByIdWithDetailsAsync(Guid id);
    Task<List<JobSummaryDto>> SearchJobsAsync(JobSearchRequest request);
    Task<List<Job>> GetJobsByCustomerAsync(Guid customerId, int pageNumber = 1, int pageSize = 20);
    Task<List<Job>> GetJobsByTradieAsync(Guid tradieId, int pageNumber = 1, int pageSize = 20);
    Task<Job> CreateAsync(Job job);
    Task<Job> UpdateAsync(Job job);
    Task<Job> UpdateTrackedAsync(Guid id, Action<Job> updateAction);
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetActiveJobsCountByCustomerAsync(Guid customerId);
    Task<List<Job>> GetJobsNeedingAttentionAsync();
    Task<List<Job>> GetExpiredJobsAsync();
}

