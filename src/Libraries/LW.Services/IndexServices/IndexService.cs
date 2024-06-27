using AutoMapper;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Shared.DTOs.Index;
using LW.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;

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
}