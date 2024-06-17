using AutoMapper;
using LW.Data.Entities;
using LW.Shared.DTOs.Admin;
using LW.Shared.DTOs.Grade;
using LW.Shared.DTOs.Level;
using Microsoft.AspNetCore.Identity;
using LW.Shared.DTOs.User;

namespace LW.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterAdminResponseDto, ApplicationUser>().ReverseMap();
        CreateMap<ApplicationUser, AdminDto>()
            .ForMember(x => x.Name, y => y.MapFrom(src => src.FirstName + " " + src.LastName))
            .ReverseMap();
        CreateMap<ApplicationUser, RegisterUserDto>().ReverseMap();
        // level
        CreateMap<Level, LevelDtoForCreate>().ReverseMap();
        CreateMap<Level, LevelDtoForUpdate>().ReverseMap();
        CreateMap<Level, LevelDto>().ReverseMap();
        //Grade
        CreateMap<Grade, GradeDto>().ReverseMap();
        CreateMap<Grade, GradeCreateDto>().ReverseMap();
        CreateMap<Grade, GradeUpdateDto>().ReverseMap();
    }
}