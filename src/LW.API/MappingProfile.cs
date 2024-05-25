using AutoMapper;
using LW.Data.Entities;
using LW.Shared.DTOs.User;

namespace LW.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, RegisterUserDto>().ReverseMap();
    }
}