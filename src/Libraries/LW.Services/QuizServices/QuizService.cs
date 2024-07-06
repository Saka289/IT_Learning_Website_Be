using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.DTOs.Quiz;
using LW.Shared.SeedWork;
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

    public async Task<ApiResult<IEnumerable<QuizDto>>> GetAllQuizByTopicId(int topicId)
    {
        var quizList = await _quizRepository.GetAllQuizByTopicId(topicId);
        if (!quizList.Any())
        {
            return new ApiResult<IEnumerable<QuizDto>>(false, "Quiz is null !!!");
        }

        var result = _mapper.Map<IEnumerable<QuizDto>>(quizList);
        return new ApiSuccessResult<IEnumerable<QuizDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<QuizDto>>> GetAllQuizByLessonId(int lessonId)
    {
        var quizList = await _quizRepository.GetAllQuizByLessonId(lessonId);
        if (!quizList.Any())
        {
            return new ApiResult<IEnumerable<QuizDto>>(false, "Quiz is null !!!");
        }

        var result = _mapper.Map<IEnumerable<QuizDto>>(quizList);
        return new ApiSuccessResult<IEnumerable<QuizDto>>(result);
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
        return new ApiSuccessResult<bool>(true, "Quiz update successfully !!!");
    }

    public async Task<ApiResult<QuizDto>> CreateQuiz(QuizCreateDto quizCreateDto)
    {
        if (quizCreateDto.LessonId > 0)
        {
            var lessonEntity = await _lessonRepository.GetLessonById(Convert.ToInt32(quizCreateDto.LessonId));
            if (lessonEntity == null)
            {
                return new ApiResult<QuizDto>(false, "Lesson is null !!!");
            }
        }
        else
        {
            var topicEntity = await _topicRepository.GetTopicById(Convert.ToInt32(quizCreateDto.TopicId));
            if (topicEntity == null)
            {
                return new ApiResult<QuizDto>(false, "Topic is null !!!");
            }

        }
        var quizEntity = _mapper.Map<Quiz>(quizCreateDto);
        quizEntity.KeyWord = quizCreateDto.Title.RemoveDiacritics();
        var quizCreate = await _quizRepository.CreateQuiz(quizEntity);
        var result = _mapper.Map<QuizDto>(quizCreate);
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

        return new ApiResult<bool>(true, "Delete Quiz successfully !!!");
    }
}