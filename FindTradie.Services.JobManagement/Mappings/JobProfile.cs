// ===== AUTOMAPPER PROFILES =====
// Mappings/JobProfile.cs
using AutoMapper;
using FindTradie.Services.JobManagement.Entities;
using FindTradie.Services.JobManagement.DTOs;
using Microsoft.AspNetCore.SignalR.Protocol;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using System.Linq;

namespace FindTradie.Services.JobManagement.Mappings;

public class JobProfile : Profile
{
    public JobProfile()
    {
        // Job mappings
        CreateMap<Job, JobDetailDto>()
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.OrderBy(i => i.DisplayOrder)))
            .ForMember(dest => dest.Quotes, opt => opt.MapFrom(src => src.Quotes.Where(q => !q.IsDeleted)))
            .ForMember(dest => dest.RecentMessages, opt => opt.MapFrom(src => src.Messages.Where(m => !m.IsDeleted).OrderByDescending(m => m.CreatedAt).Take(5)));

        CreateMap<Job, JobSummaryDto>()
            .ForMember(dest => dest.QuoteCount, opt => opt.MapFrom(src => src.Quotes.Count(q => !q.IsDeleted)))
            .ForMember(dest => dest.HasImages, opt => opt.MapFrom(src => src.Images.Any()))
            .ForMember(dest => dest.DistanceKm, opt => opt.Ignore());

        // Quote mappings
        CreateMap<Quote, QuoteDetailDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<Quote, QuoteSummaryDto>()
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.ExpiresAt < DateTime.UtcNow));

        // Other entity mappings
        CreateMap<QuoteItem, QuoteItemDto>();
        CreateMap<JobImage, JobImageDto>();
        CreateMap<JobMessage, JobMessageDto>();

        // Reverse mappings
        CreateMap<CreateJobRequest, Job>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.Quotes, opt => opt.Ignore())
            .ForMember(dest => dest.Messages, opt => opt.Ignore())
            .ForMember(dest => dest.StatusHistory, opt => opt.Ignore());

        CreateMap<CreateQuoteRequest, Quote>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Items, opt => opt.Ignore())
            .ForMember(dest => dest.Job, opt => opt.Ignore());
    }
}