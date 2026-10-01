using AutoMapper;
using IdentityManagement.Application.Features.Authentication.Dtos;
using IdentityManagement.Domain.Entities;

namespace IdentityManagement.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, UserProfileDto>()
            .ForCtorParam(nameof(UserProfileDto.PhoneConfirmed), o => o.MapFrom(s => s.PhoneConfirmed));

        CreateMap<RegisterRequestDto, ApplicationUser>()
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PhoneNumber))
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email));
    }
}
