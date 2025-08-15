// Services/TradieService.cs
using AutoMapper;
using FindTradie.Services.TradieManagement.Entities;
using FindTradie.Services.TradieManagement.DTOs;
using FindTradie.Services.TradieManagement.Repositories;
using FindTradie.Shared.Domain.Enums;
using FindTradie.Shared.Contracts.Common;
using TradieServiceEntity = FindTradie.Services.TradieManagement.Entities.TradieService; 


namespace FindTradie.Services.TradieManagement.Services;

public class TradieService : ITradieService
{
    private readonly ITradieRepository _tradieRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TradieService> _logger;
    private string Description;

    public ServiceCategory Category { get; private set; }
    public string SubCategory { get; private set; }

    public TradieService(
        ITradieRepository tradieRepository,
        IMapper mapper,
        ILogger<TradieService> logger)
    {
        _tradieRepository = tradieRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<TradieProfileDetailDto>> CreateProfileAsync(CreateTradieProfileRequest request)
    {
        try
        {
            // Check if ABN is unique
            if (!await _tradieRepository.IsABNUniqueAsync(request.ABN))
            {
                return ApiResponse<TradieProfileDetailDto>.ErrorResult(
                    "ABN already registered with another tradie profile");
            }

            // Check if user already has a tradie profile
            var existingProfile = await _tradieRepository.GetByUserIdAsync(request.UserId);
            if (existingProfile != null)
            {
                return ApiResponse<TradieProfileDetailDto>.ErrorResult(
                    "User already has a tradie profile");
            }

            var profile = new TradieProfile
            {
                UserId = request.UserId,
                BusinessName = request.BusinessName,
                ABN = request.ABN,
                ACN = request.ACN,
                Description = request.Description,
                HourlyRate = request.HourlyRate,
                ServiceRadius = request.ServiceRadius,
                IsEmergencyService = request.IsEmergencyService,
                InsuranceProvider = request.InsuranceProvider,
                InsurancePolicyNumber = request.InsurancePolicyNumber,
                InsuranceExpiryDate = request.InsuranceExpiryDate,
                VerificationStatus = VerificationStatus.Pending
            };

            // Add services
            foreach (var category in request.ServiceCategories)
            {
                profile.Services.Add(new TradieServiceEntity
                {
                    Category = category,
                    SubCategory = category.ToString(),
                    Description = $"{category} services"
                });
            }

            var createdProfile = await _tradieRepository.CreateAsync(profile);
            var dto = _mapper.Map<TradieProfileDetailDto>(createdProfile);

            _logger.LogInformation("Created tradie profile {ProfileId} for user {UserId}",
                createdProfile.Id, request.UserId);

            return ApiResponse<TradieProfileDetailDto>.SuccessResult(dto,
                "Tradie profile created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tradie profile for user {UserId}", request.UserId);
            return ApiResponse<TradieProfileDetailDto>.ErrorResult(
                "Failed to create tradie profile");
        }
    }

    public async Task<ApiResponse<TradieProfileDetailDto>> GetProfileAsync(Guid id)
    {
        try
        {
            var profile = await _tradieRepository.GetByIdAsync(id);
            if (profile == null)
            {
                return ApiResponse<TradieProfileDetailDto>.ErrorResult("Tradie profile not found");
            }

            var dto = _mapper.Map<TradieProfileDetailDto>(profile);
            return ApiResponse<TradieProfileDetailDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tradie profile {ProfileId}", id);
            return ApiResponse<TradieProfileDetailDto>.ErrorResult(
                "Failed to retrieve tradie profile");
        }
    }

    public async Task<ApiResponse<TradieProfileDetailDto>> GetProfileByUserIdAsync(Guid userId)
    {
        try
        {
            var profile = await _tradieRepository.GetByUserIdAsync(userId);
            if (profile == null)
            {
                return ApiResponse<TradieProfileDetailDto>.ErrorResult("Tradie profile not found");
            }

            var dto = _mapper.Map<TradieProfileDetailDto>(profile);
            return ApiResponse<TradieProfileDetailDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tradie profile for user {UserId}", userId);
            return ApiResponse<TradieProfileDetailDto>.ErrorResult(
                "Failed to retrieve tradie profile");
        }
    }

    public async Task<ApiResponse<TradieProfileDetailDto>> UpdateProfileAsync(Guid id, UpdateTradieProfileRequest request)
    {
        try
        {
            var profile = await _tradieRepository.GetByIdAsync(id);
            if (profile == null)
            {
                return ApiResponse<TradieProfileDetailDto>.ErrorResult("Tradie profile not found");
            }

            // Update profile properties
            profile.BusinessName = request.BusinessName;
            profile.Description = request.Description;
            profile.HourlyRate = request.HourlyRate;
            profile.ServiceRadius = request.ServiceRadius;
            profile.IsEmergencyService = request.IsEmergencyService;
            profile.IsAvailable = request.IsAvailable;

            var updatedProfile = await _tradieRepository.UpdateAsync(profile);
            var dto = _mapper.Map<TradieProfileDetailDto>(updatedProfile);

            return ApiResponse<TradieProfileDetailDto>.SuccessResult(dto,
                "Tradie profile updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tradie profile {ProfileId}", id);
            return ApiResponse<TradieProfileDetailDto>.ErrorResult(
                "Failed to update tradie profile");
        }
    }

    public async Task<ApiResponse<List<TradieProfileSummaryDto>>> SearchTradiesAsync(SearchTradiesRequest request)
    {
        try
        {
            var results = await _tradieRepository.SearchTradiesAsync(request);
            return ApiResponse<List<TradieProfileSummaryDto>>.SuccessResult(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tradies");
            return ApiResponse<List<TradieProfileSummaryDto>>.ErrorResult(
                "Failed to search tradies");
        }
    }

    public async Task<ApiResponse<bool>> UpdateAvailabilityAsync(Guid id, bool isAvailable, DateTime? availableFrom = null)
    {
        try
        {
            var profile = await _tradieRepository.GetByIdAsync(id);
            if (profile == null)
            {
                return ApiResponse<bool>.ErrorResult("Tradie profile not found");
            }

            profile.IsAvailable = isAvailable;
            profile.AvailableFrom = availableFrom;

            await _tradieRepository.UpdateAsync(profile);

            return ApiResponse<bool>.SuccessResult(true, "Availability updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating availability for tradie {ProfileId}", id);
            return ApiResponse<bool>.ErrorResult("Failed to update availability");
        }
    }

    public async Task<ApiResponse<bool>> VerifyTradieAsync(Guid id, VerificationStatus status, string? notes = null)
    {
        try
        {
            var profile = await _tradieRepository.GetByIdAsync(id);
            if (profile == null)
            {
                return ApiResponse<bool>.ErrorResult("Tradie profile not found");
            }

            profile.VerificationStatus = status;
            // You might want to add a verification history or notes field

            await _tradieRepository.UpdateAsync(profile);

            _logger.LogInformation("Updated verification status for tradie {ProfileId} to {Status}",
                id, status);

            return ApiResponse<bool>.SuccessResult(true, "Verification status updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating verification status for tradie {ProfileId}", id);
            return ApiResponse<bool>.ErrorResult("Failed to update verification status");
        }
    }
}