using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs.Quiz;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using MockQueryable.Moq;
using Serilog;

namespace LW.Services.QuizServices;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IMapper _mapper;
    private readonly IElasticSearchService<QuizDto, int> _elasticSearchService;
    private readonly ILogger _logger;

    public QuizService(IQuizRepository quizRepository, IMapper mapper,
        IElasticSearchService<QuizDto, int> elasticSearchService, ILogger logger, ILessonRepository lessonRepository,
        ITopicRepository topicRepository)
    {
        _quizRepository = quizRepository;
        _mapper = mapper;
        _elasticSearchService = elasticSearchService;
        _logger = logger;
        _lessonRepository = lessonRepository;
        _topicRepository = topicRepository;
    }

    public async Task<ApiResult<IEnumerable<QuizDto>>> GetAllQuiz()
    {
        var quizList = await _quizRepository.GetAllQuiz();
        if (!quizList.Any())
        {
            return new ApiResult<IEnumerable<QuizDto>>(false, "Quiz is null !!!");
        }

        var result = _mapper.Map<IEnumerable<QuizDto>>(quizList);
        return new ApiSuccessResult<IEnumerable<QuizDto>>(result);
    }

    public async Task<ApiResult<PagedList<QuizDto>>> GetAllQuizPagination(ETypeQuiz typeQuiz,
        PagingRequestParameters pagingRequestParameters)
    {
        var quizList = await _quizRepository.GetAllQuizPagination();
        if (!quizList.Any())
        {
            return new ApiResult<PagedList<QuizDto>>(false, "Quiz is null !!!");
        }

        if (typeQuiz > 0)
        {
            quizList = quizList.Where(q => q.Type == typeQuiz);
        }

        var result = _mapper.ProjectTo<QuizDto>(quizList);
        var pagedResult = await PagedList<QuizDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);
        return new ApiSuccessResult<PagedList<QuizDto>>(pagedResult);
    }

    public async Task<ApiResult<PagedList<QuizDto>>> GetAllQuizByTopicIdPagination(int topicId, ETypeQuiz typeQuiz,
        PagingRequestParameters pagingRequestParameters)
    {
        var quizList = await _quizRepository.GetAllQuizByTopicIdPagination(topicId);
        if (!quizList.Any())
        {
            return new ApiResult<PagedList<QuizDto>>(false, "Quiz is null !!!");
        }

        if (typeQuiz > 0)
        {
            quizList = quizList.Where(q => q.Type == typeQuiz);
        }

        var result = _mapper.ProjectTo<QuizDto>(quizList);
        var pagedResult = await PagedList<QuizDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);
        return new ApiSuccessResult<PagedList<QuizDto>>(pagedResult);
    }

    public async Task<ApiResult<PagedList<QuizDto>>> GetAllQuizByLessonIdPagination(int lessonId, ETypeQuiz typeQuiz,
        PagingRequestParameters pagingRequestParameters)
    {
        var quizList = await _quizRepository.GetAllQuizByLessonIdPagination(lessonId);
        if (!quizList.Any())
        {
            return new ApiResult<PagedList<QuizDto>>(false, "Quiz is null !!!");
        }

        if (typeQuiz > 0)
        {
            quizList = quizList.Where(q => q.Type == typeQuiz);
        }

        var result = _mapper.ProjectTo<QuizDto>(quizList);
        var pagedResult = await PagedList<QuizDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);
        return new ApiSuccessResult<PagedList<QuizDto>>(pagedResult);
    }

    public async Task<ApiResult<PagedList<QuizDto>>> SearchQuizPagination(SearchQuizDto searchQuizDto)
    {
        var quizEntity = await _elasticSearchService.SearchDocumentAsync(ElasticConstant.ElasticQuizzes, searchQuizDto);
        if (quizEntity is null)
        {
            return new ApiResult<PagedList<QuizDto>>(false, $"Lesson not found by {searchQuizDto.Key} !!!");
        }

        if (searchQuizDto.TopicId > 0)
        {
            quizEntity = quizEntity.Where(t => t.TopicId == searchQuizDto.TopicId).ToList();
        }

        if (searchQuizDto.LessonId > 0)
        {
            quizEntity = quizEntity.Where(t => t.LessonId == searchQuizDto.LessonId).ToList();
        }

        if (searchQuizDto.Type > 0)
        {
            quizEntity = quizEntity.Where(q => q.Type.Equals(searchQuizDto.Type.ToString()));
        }

        var result = _mapper.Map<IEnumerable<QuizDto>>(quizEntity);
        var pagedResult = await PagedList<QuizDto>.ToPageListAsync(result.AsQueryable().BuildMock(),
            searchQuizDto.PageIndex, searchQuizDto.PageSize, searchQuizDto.OrderBy, searchQuizDto.IsAscending);
        return new ApiSuccessResult<PagedList<QuizDto>>(pagedResult);
    }

    public async Task<ApiResult<QuizDto>> GetQuizById(int id)
    {
        var quizEntity = await _quizRepository.GetQuizById(id);
        if (quizEntity == null)
        {
            return new ApiResult<QuizDto>(false, "Quiz is null !!!");
        }

        var result = _mapper.Map<QuizDto>(quizEntity);
        return new ApiSuccessResult<QuizDto>(result);
    }

    public async Task<ApiResult<bool>> UpdateQuizStatus(int id)
    {
        var quizEntity = await _quizRepository.GetQuizById(id);
        if (quizEntity is null)
        {
            return new ApiResult<bool>(false, "Quiz not found !!!");
        }

        quizEntity.IsActive = !quizEntity.IsActive;
        await _quizRepository.UpdateQuiz(quizEntity);
        var result = _mapper.Map<QuizDto>(quizEntity);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticQuizzes, result, id);
        return new ApiSuccessResult<bool>(true, "Quiz update successfully !!!");
    }

    public async Task<ApiResult<QuizDto>> CreateQuiz(QuizCreateDto quizCreateDto)
    {
        var lessonEntity = new Lesson();
        var topicEntity = new Topic();
        if (quizCreateDto.LessonId > 0)
        {
            lessonEntity = await _lessonRepository.GetLessonById(Convert.ToInt32(quizCreateDto.LessonId));
            if (lessonEntity == null)
            {
                return new ApiResult<QuizDto>(false, "Lesson is null !!!");
            }
        }
        else
        {
            topicEntity = await _topicRepository.GetTopicById(Convert.ToInt32(quizCreateDto.TopicId));
            if (topicEntity == null)
            {
                return new ApiResult<QuizDto>(false, "Topic is null !!!");
            }
        }

        var quizEntity = _mapper.Map<Quiz>(quizCreateDto);
        quizEntity.KeyWord = quizCreateDto.Title.RemoveDiacritics();
        var quizCreate = await _quizRepository.CreateQuiz(quizEntity);
        quizCreate.Lesson = lessonEntity;
        quizCreate.Topic = topicEntity;
        var result = _mapper.Map<QuizDto>(quizCreate);
        _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticQuizzes, result, q => q.Id);
        return new ApiSuccessResult<QuizDto>(result);
    }

    public async Task<ApiResult<QuizDto>> UpdateQuiz(QuizUpdateDto quizUpdateDto)
    {
        var quizEntity = await _quizRepository.GetQuizById(quizUpdateDto.Id);
        if (quizEntity == null)
        {
            return new ApiResult<QuizDto>(false, "Quiz is null !!!");
        }

        if (quizUpdateDto.LessonId > 0)
        {
            var lessonEntity = await _lessonRepository.GetLessonById(Convert.ToInt32(quizUpdateDto.LessonId));
            if (lessonEntity == null)
            {
                return new ApiResult<QuizDto>(false, "Lesson is null !!!");
            }
        }
        else
        {
            var topicEntity = await _topicRepository.GetTopicById(Convert.ToInt32(quizUpdateDto.TopicId));
            if (topicEntity == null)
            {
                return new ApiResult<QuizDto>(false, "Topic is null !!!");
            }
        }

        var quizMapper = _mapper.Map(quizUpdateDto, quizEntity);
        quizMapper.KeyWord = quizUpdateDto.Title.RemoveDiacritics();
        var quizUpdate = await _quizRepository.UpdateQuiz(quizMapper);
        var result = _mapper.Map<QuizDto>(quizUpdate);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticQuizzes, result, quizUpdateDto.Id);
        return new ApiSuccessResult<QuizDto>(result);
    }

    public async Task<ApiResult<bool>> DeleteQuiz(int id)
    {
        var quizEntity = await _quizRepository.GetQuizById(id);
        if (quizEntity == null)
        {
            return new ApiResult<bool>(false, "Quiz is null !!!");
        }

        var quiz = await _quizRepository.DeleteQuiz(id);
        if (!quiz)
        {
            return new ApiResult<bool>(false, "Failed Delete Quiz not found !!!");
        }

        _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticQuizzes, id);
        return new ApiResult<bool>(true, "Delete Quiz successfully !!!");
    }
}