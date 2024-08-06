﻿using AutoMapper;
using LW.Data.Entities;
using LW.Infrastructure.Extensions;
using LW.Shared.DTOs;
using LW.Shared.DTOs.Admin;
using LW.Shared.DTOs.CommentDocument;
using LW.Shared.DTOs.Competition;
using LW.Shared.DTOs.Compile;
using LW.Shared.DTOs.Grade;
using LW.Shared.DTOs.Level;
using LW.Shared.DTOs.User;
using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Editorial;
using LW.Shared.DTOs.Exam;
using LW.Shared.DTOs.ExamAnswer;
using LW.Shared.DTOs.ExamCode;
using LW.Shared.DTOs.ExecuteCode;
using LW.Shared.DTOs.Index;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Problem;
using LW.Shared.DTOs.ProgramLanguage;
using LW.Shared.DTOs.Notification;
using LW.Shared.DTOs.Post;
using LW.Shared.DTOs.PostComment;
using LW.Shared.DTOs.Quiz;
using LW.Shared.DTOs.QuizAnswer;
using LW.Shared.DTOs.QuizQuestion;
using LW.Shared.DTOs.QuizQuestionRelation;
using LW.Shared.DTOs.Submission;
using LW.Shared.DTOs.Tag;
using LW.Shared.DTOs.TestCase;
using LW.Shared.DTOs.Topic;
using LW.Shared.DTOs.UserExam;
using LW.Shared.DTOs.UserQuiz;
using LW.Shared.DTOs.VoteComment;
using LW.Shared.Enums;
using LW.Shared.Solution;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
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
            .ForMember(x => x.BookCollection,
                y => y.MapFrom(src => EnumHelperExtensions.GetDisplayName(src.BookCollection).ToString()))
            .ForMember(x => x.TypeOfBook,
                y => y.MapFrom(src => EnumHelperExtensions.GetDisplayName(src.TypeOfBook).ToString()))
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
            .ForMember(x => x.FullName,
                y => y.MapFrom(src => src.ApplicationUser.FirstName + " " + src.ApplicationUser.LastName))
            .ForMember(x => x.Avatar, y => y.MapFrom(src => src.ApplicationUser.Image))
            .ReverseMap();
        CreateMap<CommentDocument, CommentDocumentUpdateDto>().ReverseMap();
        CreateMap<CommentDocument, CommentDocumentCreateDto>().ReverseMap();
        CreateMap<CommentDocument, RepliesCommentDocumentDto>()
            .ForMember(x => x.FullName,
                y => y.MapFrom(src => src.ApplicationUser.FirstName + " " + src.ApplicationUser.LastName))
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
            .ForMember(x => x.GradeName, y => y.MapFrom(y => y.Grade.Title))
            .ForMember(x => x.UserName, y => y.MapFrom(y => y.ApplicationUser.UserName))
            .ReverseMap();
        CreateMap<UserGrade, UserGradeCreateDto>()
            .ReverseMap();
        CreateMap<UserGrade, UserGradeUpdateDto>()
            .ReverseMap();
        //Exam
        CreateMap<Exam, ExamDto>()
            // .ForMember(x => x.Type,
            //         y => y.MapFrom(src => EnumHelperExtensions.GetDisplayName(src.Type).ToString()))
            .ForMember(x => x.CompetitionId, y => y.MapFrom(src => src.Competition.Id))
            .ForMember(x => x.CompetitionTitle, y => y.MapFrom(src => src.Competition.Title))
            .ReverseMap();
        CreateMap<Exam, ExamCreateDto>().ReverseMap();
        CreateMap<Exam, ExamUpdateDto>().ReverseMap();
        //ExamCode
        CreateMap<ExamCode, ExamCodeDto>()
            .ForMember(x => x.ExamTitle, y => y.MapFrom(src => src.Exam.Title))
            .ForMember(x => x.NumberQuestion, y => y.MapFrom(src => src.Exam.NumberQuestion))
            .ReverseMap();
        CreateMap<ExamCode, ExamCodeCreateDto>().ReverseMap();
        CreateMap<ExamCode, ExamCodeUpdateDto>().ReverseMap();
        //ExamAnswer
        CreateMap<ExamAnswer, ExamAnswerDto>().ReverseMap();
        CreateMap<ExamAnswer, ExamAnswerCreateDto>().ReverseMap();
        CreateMap<ExamAnswer, ExamAnswerUpdateDto>().ReverseMap();
        //UserExam
        CreateMap<UserExam, UserExamDto>()
            // .ForMember(x => x.HistoryExam, y => y.MapFrom(src => JsonConvert.DeserializeObject<List<HistoryAnswer>>(src.HistoryExam)))
            .ForMember(x => x.UserName, y => y.MapFrom(src => src.ApplicationUser.UserName))
            .ForMember(x => x.ExamName, y => y.MapFrom(src => src.ExamCode.Exam.Title))
            .ForMember(x => x.ExamId, y => y.MapFrom(src => src.ExamCode.Exam.Id))
            .ForMember(x => x.ExamCodeId, y => y.MapFrom(src => src.ExamCode.Id))
            .ForMember(x => x.Code, y => y.MapFrom(src => src.ExamCode.Code))
            .ReverseMap();
        CreateMap<HistoryAnswer, HistoryAnswerDto>().ReverseMap();
        //Quiz
        CreateMap<Quiz, QuizDto>()
            .ForMember(x => x.TopicTitle, y => y.MapFrom(src => src.Topic.Title))
            .ForMember(x => x.LessonTitle, y => y.MapFrom(src => src.Lesson.Title))
            .ForMember(x => x.TotalQuestion, y => y.MapFrom(src => src.QuizQuestionRelations.Count))
            .ReverseMap();
        CreateMap<Quiz, QuizCreateDto>().ReverseMap();
        CreateMap<Quiz, QuizUpdateDto>().ReverseMap();
        //QuizQuestion
        CreateMap<QuizQuestion, QuizQuestionDto>().ReverseMap();
        CreateMap<QuizQuestion, QuizQuestionTestDto>().ReverseMap();
        CreateMap<QuizQuestion, QuizQuestionCreateDto>().ReverseMap();
        CreateMap<QuizQuestion, QuizQuestionUpdateDto>().ReverseMap();
        CreateMap<QuizQuestion, QuizQuestionImportDto>().ReverseMap();
        CreateMap<QuizQuestionExcelDto, QuizQuestionImportDto>().ReverseMap();
        //QuizAnswer
        CreateMap<QuizAnswer, QuizAnswerDto>().ReverseMap();
        CreateMap<QuizAnswer, QuizAnswerTestDto>().ReverseMap();
        CreateMap<QuizAnswer, QuizAnswerCreateDto>().ReverseMap();
        CreateMap<QuizAnswer, QuizAnswerUpdateDto>().ReverseMap();
        //UserQuiz
        CreateMap<UserQuiz, UserQuizDto>().ReverseMap();
        CreateMap<HistoryQuiz, HistoryQuizDto>().ReverseMap();
        //Tag
        CreateMap<Tag, TagDto>().ReverseMap();
        CreateMap<Tag, TagCreateDto>().ReverseMap();
        CreateMap<Tag, TagUpdateDto>().ReverseMap();
        //QuizQuestionRelation
        CreateMap<QuizQuestionRelation, QuizQuestionRelationDto>().ReverseMap();
        // Competition
        CreateMap<Competition, CompetitionDto>().ReverseMap();
        CreateMap<Competition, CompetitionCreateDto>().ReverseMap();
        CreateMap<Competition, CompetitionUpdateDto>().ReverseMap();
        // ProgramLanguage
        CreateMap<ProgramLanguage, ProgramLanguageDto>().ReverseMap();
        CreateMap<ProgramLanguage, ProgramLanguageCreateDto>().ReverseMap();
        CreateMap<ProgramLanguage, ProgramLanguageUpdateDto>().ReverseMap();
        // Solution
        CreateMap<Solution, SolutionDto>()
            .ForMember(x => x.FullName,
                y => y.MapFrom(src => src.ApplicationUser.FirstName + " " + src.ApplicationUser.LastName))
            .ForMember(x => x.Image, y => y.MapFrom(src => src.ApplicationUser.Image))
            .ReverseMap();
        CreateMap<Solution, SolutionCreateDto>().ReverseMap();
        CreateMap<Solution, SolutionUpdateDto>().ReverseMap();
        // Problem
        CreateMap<Problem, ProblemDto>().ReverseMap();
        CreateMap<Problem, ProblemCreateDto>().ReverseMap();
        CreateMap<Problem, ProblemUpdateDto>().ReverseMap();
        // Editorial 
        CreateMap<Editorial, EditorialDto>().ReverseMap();
        CreateMap<Editorial, EditorialCreateDto>().ReverseMap();
        CreateMap<Editorial, EditorialUpdateDto>().ReverseMap();
        // TestCase
        CreateMap<TestCase, TestCaseDto>().ReverseMap();
        CreateMap<TestCase, TestCaseCreateDto>().ReverseMap();
        CreateMap<TestCase, TestCaseUpdateDto>().ReverseMap();
        // ExecuteCode
        CreateMap<ExecuteCode, ExecuteCodeDto>().ReverseMap();
        CreateMap<ExecuteCode, ExecuteCodeCreateDto>().ReverseMap();
        CreateMap<ExecuteCode, ExecuteCodeUpdateDto>().ReverseMap();
        // Submission
        CreateMap<Submission, SubmissionDto>()
            .ForMember(x => x.StatusId, y => y.MapFrom(src => (int)src.Status))
            .ReverseMap();
        //Post
        CreateMap<Post, PostDto>()
            .ForMember(x=>x.UserName, y=>y.MapFrom(src=>src.ApplicationUser.UserName))
            .ForMember(x=>x.GradeTitle, y=>y.MapFrom(src=>src.Grade.Title))
            .ForMember(x=>x.NumberOfComment, y=>y.MapFrom(src=>src.PostComments.Count()))
            .ForMember(x=>x.Avatar, y=>y.MapFrom(src=>src.ApplicationUser.Image))
            .ReverseMap();
        CreateMap<Post, PostCreateDto>().ReverseMap();
        CreateMap<Post, PostUpdateDto>().ReverseMap();
        //PostComment
        CreateMap<PostComment, PostCommentDto>()
            .ForMember(x => x.FullName,
                y => y.MapFrom(src => src.ApplicationUser.FirstName + " " + src.ApplicationUser.LastName))
            .ForMember(x => x.Avatar,
                y => y.MapFrom(src => src.ApplicationUser.Image))
            .ForMember(x=>x.NumberOfReply, y=>y.MapFrom(src=>src.PostCommentChilds.Count()))

            .ReverseMap();
        CreateMap<PostComment, PostCommentReplyDto>()
            .ForMember(x => x.FullName,
                y => y.MapFrom(src => src.ApplicationUser.FirstName + " " + src.ApplicationUser.LastName))
            .ForMember(x => x.Avatar,
                y => y.MapFrom(src => src.ApplicationUser.Image))
            .ReverseMap();
        CreateMap<PostComment, PostCommentCreateDto>().ReverseMap();
        CreateMap<PostComment, PostCommentUpdateDto>().ReverseMap();
        // Notification
        CreateMap<Notification, NotificationCreateDto>().ReverseMap();
        CreateMap<Notification, NotificationDto>().ReverseMap();
        //VoteComment
        CreateMap<VoteComment, VoteCommentDto>().ReverseMap();
        CreateMap<VoteComment, VoteCommentCreateDto>().ReverseMap();
        CreateMap<VoteComment, VoteCommentUpdateDto>().ReverseMap();
    }
}