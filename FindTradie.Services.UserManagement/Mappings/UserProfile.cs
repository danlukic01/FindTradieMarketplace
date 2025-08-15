// Mappings/UserProfile.cs
using AutoMapper;
using FindTradie.Services.UserManagement.Entities;
using FindTradie.Shared.Contracts.DTOs;

namespace FindTradie.Services.UserManagement.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // User mappings
        CreateMap<User, UserProfileDto>()
            .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.IsEmailVerified && src.IsPhoneVerified));

        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.IsEmailVerified, opt => opt.Ignore())
            .ForMember(dest => dest.IsPhoneVerified, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
            .ForMember(dest => dest.Addresses, opt => opt.Ignore())
            .ForMember(dest => dest.Profile, opt => opt.Ignore());
    }
}