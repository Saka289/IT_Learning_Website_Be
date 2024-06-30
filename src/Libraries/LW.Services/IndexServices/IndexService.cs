using AutoMapper;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Shared.DTOs.Index;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace LW.Services.IndexServices;

public class IndexService : IIndexService
{
    private readonly IMapper _mapper;
    private readonly IDocumentRepository _documentRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly ILessonRepository _lessonRepository;

    public IndexService(IDocumentRepository documentRepository, IMapper mapper, ITopicRepository topicRepository,
        ILessonRepository lessonRepository)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
        _topicRepository = topicRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<ApiResult<DocumentIndexByDocumentDto>> GetAllDocumentIndex(int documentId)
    {
        var document = await _documentRepository.GetAllDocumentIndex(documentId);
        if (document == null)
        {
            return new ApiResult<DocumentIndexByDocumentDto>(false, "Document not found !!!");
        }

        var result = _mapper.Map<DocumentIndexByDocumentDto>(document);
        return new ApiSuccessResult<DocumentIndexByDocumentDto>(result);
    }
    public async Task<ApiResult<ETopicIndex>> CheckTopicById(int topicId)
    {
        var result = await _topicRepository.GetTopicByAllId(topicId);
        if (result == null)
        {
            return new ApiResult<ETopicIndex>(false, ETopicIndex.NotFound, "Topic not found !!!");
        }

        if (result.ParentId != null)
        {
            return new ApiSuccessResult<ETopicIndex>(ETopicIndex.ParentTopic);
        }

        return new ApiSuccessResult<ETopicIndex>(ETopicIndex.Topic);
    }

    public async Task<ApiResult<DocumentIndexByTopicDto>> GetAllTopicIndex(int topicId)
    {
        var topic = await _topicRepository.GetAllTopicIndex(topicId);
        if (topic == null)
        {
            return new ApiResult<DocumentIndexByTopicDto>(false, "Topic not found !!!");
        }

        var result = _mapper.Map<DocumentIndexByTopicDto>(topic);
        return new ApiSuccessResult<DocumentIndexByTopicDto>(result);
    }

    public async Task<ApiResult<DocumentIndexByTopicParentDto>> GetAllTopicParentIndex(int topicParentId)
    {
        var topic = await _topicRepository.GetAllTopicIndex(topicParentId);
        if (topic == null)
        {
            return new ApiResult<DocumentIndexByTopicParentDto>(false, "TopicParent not found !!!");
        }

        var result = _mapper.Map<DocumentIndexByTopicParentDto>(topic);
        return new ApiSuccessResult<DocumentIndexByTopicParentDto>(result);
    }

    public async Task<ApiResult<ELessonIndex>> CheckLessonById(int topicId)
    {
        var result = await _lessonRepository.GetAllLessonIndex(topicId);
        if (result == null)
        {
            return new ApiResult<ELessonIndex>(false, ELessonIndex.NotFound, "Lesson not found !!!");
        }

        if (result.Topic.ParentId != null)
        {
            return new ApiSuccessResult<ELessonIndex>(ELessonIndex.ParentTopic);
        }

        return new ApiSuccessResult<ELessonIndex>(ELessonIndex.Topic);
    }

    public async Task<ApiResult<DocumentIndexByLessonDto>> GetAllLessonIndex(int lessonId)
    {
        var lesson = await _lessonRepository.GetAllLessonIndex(lessonId);
        if (lesson == null)
        {
            return new ApiResult<DocumentIndexByLessonDto>(false, "Lesson not found !!!");
        }

        var result = _mapper.Map<DocumentIndexByLessonDto>(lesson);
        return new ApiSuccessResult<DocumentIndexByLessonDto>(result);
    }

    public async Task<ApiResult<DocumentIndexByLessonParentTopicDto>> GetAllLessonParentTopicIndex(int lessonId)
    {
        var lesson = await _lessonRepository.GetAllLessonIndex(lessonId);
        if (lesson == null)
        {
            return new ApiResult<DocumentIndexByLessonParentTopicDto>(false, "LessonParent not found !!!");
        }

        var result = _mapper.Map<DocumentIndexByLessonParentTopicDto>(lesson);
        return new ApiSuccessResult<DocumentIndexByLessonParentTopicDto>(result);
    }
}