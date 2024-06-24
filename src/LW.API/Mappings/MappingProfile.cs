using AutoMapper;
using LW.Data.Entities;
using LW.Shared.DTOs.Admin;
using LW.Shared.DTOs.Grade;
using LW.Shared.DTOs.Level;
using LW.Shared.DTOs.User;
using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Topic;
using Microsoft.AspNetCore.Identity;

namespace LW.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterMemberResponseDto, ApplicationUser>().ReverseMap();
        CreateMap<ApplicationUser, AdminDto>()
            .ForMember(x => x.Name, y => y.MapFrom(src => src.FirstName + " " + src.LastName))
            .ReverseMap();
        CreateMap<ApplicationUser, RegisterUserDto>().ReverseMap();
        CreateMap<ApplicationUser, UserResponseDto>()
            .ForMember(x => x.Dob, y => y.MapFrom(src => src.Dob.ToString()))
            .ReverseMap();
        // level
        CreateMap<Level, LevelDtoForCreate>().ReverseMap();
        CreateMap<Level, LevelDtoForUpdate>().ReverseMap();
        CreateMap<Level, LevelDto>().ReverseMap();
        //Grade
        CreateMap<Grade, GradeDto>().ReverseMap();
        CreateMap<Grade, GradeCreateDto>().ReverseMap();
        CreateMap<Grade, GradeUpdateDto>().ReverseMap();
        //Document 
        CreateMap<Document, DocumentDto>()
            .ForMember(x => x.GradeId, y => y.MapFrom(src => src.GradeId))
            .ForMember(x => x.GradeTitle, y => y.MapFrom(src => src.Grade.Title))
            .ReverseMap();
        CreateMap<Document, DocumentCreateDto>().ReverseMap();
        CreateMap<Document, DocumentUpdateDto>().ReverseMap();
        //Topic 
        CreateMap<Topic, TopicDto>()
            .ForMember(x => x.DocumentId, y => y.MapFrom(src => src.DocumentId))
            .ForMember(x => x.DocumentTitle, y => y.MapFrom(src => src.Document.Title))
            .ReverseMap();
        CreateMap<Topic, TopicCreateDto>().ReverseMap();
        CreateMap<Topic, TopicUpdateDto>().ReverseMap();
        //Lesson
        CreateMap<Lesson, LessonDto>()
            .ForMember(x => x.TopicId, y => y.MapFrom(src => src.TopicId))
            .ForMember(x => x.TopicTitle, y => y.MapFrom(src => src.Topic.Title))
            .ReverseMap();
        CreateMap<Lesson, LessonCreateDto>().ReverseMap();
        CreateMap<LessonUpdateDto, Lesson>()
            .ForMember(x => x.FilePath, y => y.Ignore())
            .ReverseMap();
        //Role
        CreateMap<IdentityRole, RoleDto>().ReverseMap();
    }
}