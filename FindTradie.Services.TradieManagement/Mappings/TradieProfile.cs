// ===== AUTOMAPPER PROFILES =====
// Mappings/TradieProfile.cs
using AutoMapper;
using FindTradie.Services.TradieManagement.Entities;
using FindTradie.Services.TradieManagement.DTOs;

namespace FindTradie.Services.TradieManagement.Mappings;

public class TradieProfile : Profile
{
    public TradieProfile()
    {
        // TradieProfile mappings
        CreateMap<Entities.TradieProfile, TradieProfileDetailDto>()
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services))
            .ForMember(dest => dest.Portfolio, opt => opt.MapFrom(src => src.Portfolio))
            .ForMember(dest => dest.Availability, opt => opt.MapFrom(src => src.Availability));

        CreateMap<Entities.TradieProfile, TradieProfileSummaryDto>()
            .ForMember(dest => dest.ServiceCategories,
                opt => opt.MapFrom(src => src.Services.Select(s => s.Category).Distinct().ToList()))
            .ForMember(dest => dest.DistanceKm, opt => opt.Ignore());

        // TradieService mappings
        CreateMap<Entities.TradieService, TradieServiceDto>();

        // PortfolioItem mappings
        CreateMap<PortfolioItem, PortfolioItemDto>();

        // TradieAvailability mappings
        CreateMap<TradieAvailability, TradieAvailabilityDto>();

        // Reverse mappings for creating entities
        CreateMap<CreateTradieProfileRequest, Entities.TradieProfile>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Services, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.Ignore())
            .ForMember(dest => dest.Portfolio, opt => opt.Ignore())
            .ForMember(dest => dest.Availability, opt => opt.Ignore())
            .ForMember(dest => dest.ServiceLocations, opt => opt.Ignore());
    }
}