using AutoMapper;
using LW.Data.Entities;
using LW.Shared.DTOs.Admin;
using Microsoft.AspNetCore.Identity;

namespace LW.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterAdminResponseDto, ApplicationUser>().ReverseMap();
        CreateMap<ApplicationUser, AdminDto>()
            .ForMember(x=>x.Name,y=>y.MapFrom(src=>src.FirstName+" " + src.LastName))
            .ReverseMap();
    }
}