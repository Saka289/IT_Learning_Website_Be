using AutoMapper;
using LW.Data.Entities;
using LW.Shared.DTOs.Admin;
using LW.Shared.DTOs.Grade;
using LW.Shared.DTOs.Level;
using LW.Shared.DTOs.User;
using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Topic;

namespace LW.API.Mappings;

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
        //Document 
        CreateMap<Document, DocumentDto>().ReverseMap();
        CreateMap<Document, DocumentCreateDto>().ReverseMap();
        CreateMap<Document, DocumentUpdateDto>().ReverseMap();
        //Topic 
        CreateMap<Topic, TopicDto>().ReverseMap();
        CreateMap<Topic, TopicCreateDto>().ReverseMap();
        CreateMap<Topic, TopicUpdateDto>().ReverseMap();
        //Lesson
        CreateMap<Lesson, LessonDto>().ReverseMap();
        CreateMap<Lesson, LessonCreateDto>().ReverseMap();
        CreateMap<Lesson, LessonUpdateDto>().ReverseMap();
    }
}