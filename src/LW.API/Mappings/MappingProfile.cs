using AutoMapper;
using LW.Data.Entities;
using LW.Infrastructure.Extensions;
using LW.Shared.DTOs;
using LW.Shared.DTOs.Admin;
using LW.Shared.DTOs.CommentDocumentDto;
using LW.Shared.DTOs.Grade;
using LW.Shared.DTOs.Level;
using LW.Shared.DTOs.User;
using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Exam;
using LW.Shared.DTOs.ExamAnswer;
using LW.Shared.DTOs.Index;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Topic;
using LW.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Extensions;
using ConfigurationExtensions = LW.Infrastructure.Extensions.ConfigurationExtensions;

namespace LW.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterMemberResponseDto, ApplicationUser>().ReverseMap();
        CreateMap<ApplicationUser, AdminDto>()
            .ForMember(x => x.FullName, y => y.MapFrom(src => src.FirstName + " " + src.LastName))
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
        CreateMap<Grade, GradeDto>()
            .ForMember(x => x.LevelId, y => y.MapFrom(src => src.LevelId))
            .ForMember(x => x.LevelTitle, y => y.MapFrom(src => src.Level.Title))
            .ReverseMap();
        CreateMap<Grade, GradeCreateDto>().ReverseMap();
        CreateMap<Grade, GradeUpdateDto>().ReverseMap();
        //Document 
        CreateMap<Document, DocumentDto>()
            .ForMember(x => x.GradeId, y => y.MapFrom(src => src.GradeId))
            .ForMember(x => x.GradeTitle, y => y.MapFrom(src => src.Grade.Title))
            .ForMember(x => x.BookCollection, y => y.MapFrom(src => EnumHelperExtensions.GetDisplayName(src.BookCollection).ToString()))
            .ForMember(x => x.TypeOfBook, y => y.MapFrom(src => EnumHelperExtensions.GetDisplayName(src.TypeOfBook).ToString()))
            .ReverseMap();
        CreateMap<Document, DocumentCreateDto>().ReverseMap();
        CreateMap<Document, DocumentUpdateDto>().ReverseMap();
        //Topic 
        CreateMap<Topic, TopicDto>()
            .ForMember(x => x.DocumentId, y => y.MapFrom(src => src.DocumentId))
            .ForMember(x => x.DocumentTitle, y => y.MapFrom(src => src.Document.Title))
            .ReverseMap();
        CreateMap<Topic, ChildTopicDto>()
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
        //CommentDocument
        CreateMap<CommentDocument, CommentDocumentDto>()
            .ForMember(x => x.FullName, y => y.MapFrom(src => src.ApplicationUser.FirstName + " " + src.ApplicationUser.LastName))
            .ForMember(x => x.Avatar, y => y.MapFrom(src => src.ApplicationUser.Image))
            .ReverseMap();
        CreateMap<CommentDocument, CommentDocumentUpdateDto>().ReverseMap();
        CreateMap<CommentDocument, CommentDocumentCreateDto>().ReverseMap();
        CreateMap<CommentDocument, RepliesCommentDocumentDto>()
            .ForMember(x => x.FullName, y => y.MapFrom(src => src.ApplicationUser.FirstName + " " + src.ApplicationUser.LastName))
            .ForMember(x => x.Avatar, y => y.MapFrom(src => src.ApplicationUser.Image))
            .ReverseMap();
        //IndexDocument
        CreateMap<Document, DocumentIndexByDocumentDto>()
            .ReverseMap();
        CreateMap<Topic, TopicIndexByDocumentDto>().ReverseMap();
        CreateMap<Topic, ChildTopicIndexByDocumentDto>().ReverseMap();
        CreateMap<Lesson, LessonIndexByDocumentDto>().ReverseMap();
        //IndexLesson
        CreateMap<Document, DocumentIndexByLessonDto>().ReverseMap();
        CreateMap<Topic, TopicIndexByLessonDto>().ReverseMap();
        CreateMap<Topic, ChildTopicIndexByLessonDto>().ReverseMap();
        CreateMap<Lesson, LessonIndexByLessonDto>().ReverseMap();
        CreateMap<DocumentIndexByLessonDto, Lesson>()
            .ForMember(x => x.Id, y => y.MapFrom(y => y.Topic.Lesson.Id))
            .ForMember(x => x.Title, y => y.MapFrom(y => y.Topic.Lesson.Title))
            .ForPath(x => x.Topic, y => y.MapFrom(y => y.Topic))
            .ForPath(x => x.Topic.Document.Id, y => y.MapFrom(y => y.Id))
            .ForPath(x => x.Topic.Document.Title, y => y.MapFrom(y => y.Title))
            .ReverseMap();
        //IndexLessonTopicParent
        CreateMap<Document, DocumentIndexByLessonParentTopicDto>().ReverseMap();
        CreateMap<Topic, TopicIndexByLessonParentTopicDto>().ReverseMap();
        CreateMap<Topic, ChildTopicIndexByLessonDto>().ReverseMap();
        CreateMap<Lesson, LessonIndexByLessonParentTopicDto>().ReverseMap();
        CreateMap<DocumentIndexByLessonParentTopicDto, Lesson>()
            .ForMember(x => x.Id, y => y.MapFrom(y => y.Topic.ParentTopic.Lesson.Id))
            .ForMember(x => x.Title, y => y.MapFrom(y => y.Topic.ParentTopic.Lesson.Title))
            .ForPath(x => x.Topic.Id, y => y.MapFrom(y => y.Topic.ParentTopic.Id))
            .ForPath(x => x.Topic.Title, y => y.MapFrom(y => y.Topic.ParentTopic.Title))
            .ForPath(x => x.Topic.ParentTopic.Id, y => y.MapFrom(y => y.Topic.Id))
            .ForPath(x => x.Topic.ParentTopic.Title, y => y.MapFrom(y => y.Topic.Title))
            .ForPath(x => x.Topic.ParentTopic.Lessons, y => y.MapFrom(y => y.Topic.Lessons))
            .ForPath(x => x.Topic.Document.Id, y => y.MapFrom(y => y.Id))
            .ForPath(x => x.Topic.Document.Title, y => y.MapFrom(y => y.Title))
            .ReverseMap();
        //IndexTopic
        CreateMap<Document, DocumentIndexByTopicDto>().ReverseMap();
        CreateMap<Topic, TopicIndexByTopicDto>().ReverseMap();
        CreateMap<Topic, ChildTopicIndexByTopicDto>().ReverseMap();
        CreateMap<Lesson, LessonIndexByTopicDto>().ReverseMap();
        CreateMap<DocumentIndexByTopicDto, Topic>()
            .ForMember(x => x.Id, y => y.MapFrom(y => y.Topic.Id))
            .ForMember(x => x.Title, y => y.MapFrom(y => y.Topic.Title))
            .ForPath(x => x.Document.Id, y => y.MapFrom(y => y.Id))
            .ForPath(x => x.Document.Title, y => y.MapFrom(y => y.Title))
            .ForMember(x => x.Lessons, y => y.MapFrom(y => y.Topic.Lessons))
            .ForMember(x => x.ChildTopics, y => y.MapFrom(y => y.Topic.ChildTopics))
            .ReverseMap();
        //IndexTopicParent
        CreateMap<Document, DocumentIndexByTopicParentDto>().ReverseMap();
        CreateMap<Topic, TopicIndexByTopicParentDto>().ReverseMap();
        CreateMap<Topic, ChildTopicIndexByTopicDto>().ReverseMap();
        CreateMap<Lesson, LessonIndexByTopicParentDto>().ReverseMap();
        CreateMap<DocumentIndexByTopicParentDto, Topic>()
            .ForPath(x => x.ParentTopic.Id, y => y.MapFrom(y => y.Topic.Id))
            .ForPath(x => x.ParentTopic.Title, y => y.MapFrom(y => y.Topic.Title))
            .ForPath(x => x.Document.Id, y => y.MapFrom(y => y.Id))
            .ForPath(x => x.Document.Title, y => y.MapFrom(y => y.Title))
            .ForMember(x => x.Lessons, y => y.MapFrom(y => y.Topic.ParentTopic.Lessons))
            .ForMember(x => x.Id, y => y.MapFrom(y => y.Topic.ParentTopic.Id))
            .ForMember(x => x.Title, y => y.MapFrom(y => y.Topic.ParentTopic.Title))
            .ForPath(x => x.ParentTopic.Lessons, y => y.MapFrom(y => y.Topic.Lessons))
            .ReverseMap();
        //UserGrade
        CreateMap<UserGrade, UserGradeDto>()
            .ForMember(x=>x.GradeName, y=>y.MapFrom(y=>y.Grade.Title))
            .ForMember(x=>x.UserName, y=> y.MapFrom(x=>x.ApplicationUser.UserName))
            .ReverseMap();
        CreateMap<UserGrade, UserGradeCreateDto>()
            .ReverseMap();
        CreateMap<UserGrade, UserGradeUpdateDto>()
            .ReverseMap();
        //Exam
        CreateMap<Exam, ExamDto>().ReverseMap();
        CreateMap<Exam, ExamCreateDto>().ReverseMap();
        CreateMap<Exam, ExamUpdateDto>().ReverseMap();
        //ExamImage
        CreateMap<ExamImage,ExamImageDto>().ReverseMap();
        CreateMap<ExamImage,ExamImageCreateDto>().ReverseMap();
        CreateMap<ExamImage,ExamImageUpdateDto>().ReverseMap();
        //ExamAnswer
        CreateMap<ExamAnswer,ExamAnswerDto>().ReverseMap();
        CreateMap<ExamAnswer,ExamAnswerCreateDto>().ReverseMap();
        CreateMap<ExamAnswer,ExamAnswerUpdateDto>().ReverseMap();

    }
}