using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.GradeRepositories;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Data.Repositories.TagRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs.Quiz;
using LW.Shared.DTOs.Tag;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Serilog;

namespace LW.Services.QuizServices;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;
    private readonly IElasticSearchService<QuizDto, int> _elasticSearchService;
    private readonly ILogger _logger;

    public QuizService(IQuizRepository quizRepository, IMapper mapper,
        IElasticSearchService<QuizDto, int> elasticSearchService, ILogger logger, ILessonRepository lessonRepository,
        ITopicRepository topicRepository, IGradeRepository gradeRepository, ITagRepository tagRepository)
    {
        _quizRepository = quizRepository;
        _mapper = mapper;
        _elasticSearchService = elasticSearchService;
        _logger = logger;
        _lessonRepository = lessonRepository;
        _topicRepository = topicRepository;
        _gradeRepository = gradeRepository;
        _tagRepository = tagRepository;
    }

    public async Task<ApiResult<IEnumerable<QuizDto>>> GetAllQuiz(bool? status)
    {
        var quizList = await _quizRepository.GetAllQuiz();
        if (quizList.All(q => q == null))
        {
            return new ApiResult<IEnumerable<QuizDto>>(false, "Quiz is null !!!");
        }

        if (status != null)
        {
            quizList = quizList.Where(q => q.IsActive == status);
        }

        var result = _mapper.Map<IEnumerable<QuizDto>>(quizList);
        return new ApiSuccessResult<IEnumerable<QuizDto>>(result);
    }

    public async Task<ApiResult<PagedList<QuizDto>>> GetAllQuizPagination(SearchQuizDto searchQuizDto)
    {
        IEnumerable<QuizDto> quizList = new List<QuizDto>();
        if (!string.IsNullOrEmpty(searchQuizDto.Value))
        {
            var quizListSearch = await _elasticSearchService.SearchDocumentFieldAsync(ElasticConstant.ElasticQuizzes,
                new SearchRequestValue
                {
                    Value = searchQuizDto.Value,
                    Size = searchQuizDto.Size,
                });
            if (quizListSearch is null)
            {
                return new ApiResult<PagedList<QuizDto>>(false, "Quiz not found !!!");
            }

            quizList = quizListSearch.ToList();
        }
        else
        {
            var quizListAll = await _quizRepository.GetAllQuiz();
            if (quizListAll.All(q => q == null))
            {
                return new ApiResult<PagedList<QuizDto>>(false, "Quiz is null !!!");
            }

            quizList = _mapper.Map<IEnumerable<QuizDto>>(quizListAll);
        }

        if (searchQuizDto.Status != null)
        {
            quizList = quizList.Where(q => q.IsActive == searchQuizDto.Status);
        }

        if (searchQuizDto.Custom == ECustomQuiz.Custom)
        {
            quizList = quizList.Where(t => t.LessonId == 0 && t.TopicId == 0);
        }
        else if (searchQuizDto.Custom == ECustomQuiz.TopicAndLesson)
        {
            quizList = quizList.Where(t => t.LessonId != 0 || t.TopicId != 0);
        }

        if (searchQuizDto.GradeId > 0)
        {
            quizList = quizList.Where(t => t.GradeId == searchQuizDto.GradeId);
        }

        if (searchQuizDto.TopicId > 0)
        {
            quizList = quizList.Where(t => t.TopicId == searchQuizDto.TopicId);
        }

        if (searchQuizDto.LessonId > 0)
        {
            quizList = quizList.Where(t => t.LessonId == searchQuizDto.LessonId);
        }

        if (searchQuizDto.Type > 0)
        {
            quizList = quizList.Where(q => q.TypeId.Equals((int)searchQuizDto.Type));
        }

        var pagedResult = await PagedList<QuizDto>.ToPageListAsync(quizList.AsQueryable().BuildMock(),
            searchQuizDto.PageIndex, searchQuizDto.PageSize, searchQuizDto.OrderBy, searchQuizDto.IsAscending);
        return new ApiSuccessResult<PagedList<QuizDto>>(pagedResult);
    }

    public async Task<ApiResult<IEnumerable<QuizDto>>> GetAllQuizNoPagination(SearchQuizDto searchQuizDto)
    {
        IEnumerable<QuizDto> quizList = new List<QuizDto>();
        if (!string.IsNullOrEmpty(searchQuizDto.Value))
        {
            var quizListSearch = await _elasticSearchService.SearchDocumentFieldAsync(ElasticConstant.ElasticQuizzes,
                new SearchRequestValue
                {
                    Value = searchQuizDto.Value,
                    Size = searchQuizDto.Size,
                });
            if (quizListSearch is null)
            {
                return new ApiResult<IEnumerable<QuizDto>>(false, "Quiz not found !!!");
            }

            quizList = quizListSearch.ToList();
        }
        else
        {
            var quizListAll = await _quizRepository.GetAllQuiz();
            if (quizListAll.All(q => q == null))
            {
                return new ApiResult<IEnumerable<QuizDto>>(false, "Quiz is null !!!");
            }

            quizList = _mapper.Map<IEnumerable<QuizDto>>(quizListAll);
        }

        if (searchQuizDto.Status != null)
        {
            quizList = quizList.Where(q => q.IsActive == searchQuizDto.Status);
        }

        if (searchQuizDto.Custom == ECustomQuiz.Custom)
        {
            quizList = quizList.Where(t => t.LessonId == 0 && t.TopicId == 0);
        }
        else if (searchQuizDto.Custom == ECustomQuiz.TopicAndLesson)
        {
            quizList = quizList.Where(t => t.LessonId != 0 || t.TopicId != 0);
        }

        if (searchQuizDto.GradeId > 0)
        {
            quizList = quizList.Where(t => t.GradeId == searchQuizDto.GradeId);
        }

        if (searchQuizDto.TopicId > 0)
        {
            quizList = quizList.Where(t => t.TopicId == searchQuizDto.TopicId);
        }

        if (searchQuizDto.LessonId > 0)
        {
            quizList = quizList.Where(t => t.LessonId == searchQuizDto.LessonId);
        }

        if (searchQuizDto.Type > 0)
        {
            quizList = quizList.Where(q => q.TypeId.Equals((int)searchQuizDto.Type));
        }

        return new ApiSuccessResult<IEnumerable<QuizDto>>(quizList);
    }

    public async Task<ApiResult<IEnumerable<TagDto>>> GetQuizIdByTag(int id)
    {
        var quiz = await _quizRepository.GetQuizById(id);
        if (quiz is null)
        {
            return new ApiResult<IEnumerable<TagDto>>(false, "Quiz not found !!!");
        }

        var listStringTag = quiz.KeyWord.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries);
        var listTag = new List<Tag>();
        foreach (var item in listStringTag)
        {
            var tagEntity = await _tagRepository.GetTagByKeyword(item);
            if (tagEntity is not null)
            {
                listTag.Add(tagEntity);
            }
        }

        var result = _mapper.Map<IEnumerable<TagDto>>(listTag);
        return new ApiSuccessResult<IEnumerable<TagDto>>(result);
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
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticQuizzes, result, id);
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

        if (quizCreateDto.TopicId > 0)
        {
            topicEntity = await _topicRepository.GetTopicById(Convert.ToInt32(quizCreateDto.TopicId));
            if (topicEntity == null)
            {
                return new ApiResult<QuizDto>(false, "Topic is null !!!");
            }
        }

        if (quizCreateDto.GradeId > 0)
        {
            var gradeEntity = await _gradeRepository.GetGradeById(Convert.ToInt32(quizCreateDto.GradeId));
            if (gradeEntity is null)
            {
                return new ApiResult<QuizDto>(false, "Grade is null");
            }
        }

        var quizEntity = _mapper.Map<Quiz>(quizCreateDto);
        quizEntity.KeyWord = (quizCreateDto.TagValues is not null)
            ? quizCreateDto.TagValues.ConvertToTagString()
            : quizCreateDto.Title.RemoveDiacritics();
        var quizCreate = await _quizRepository.CreateQuiz(quizEntity);
        await CreateOrUpdateElasticQuiz(quizCreate.Id, true);
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

        if (quizUpdateDto.TopicId > 0)
        {
            var topicEntity = await _topicRepository.GetTopicById(Convert.ToInt32(quizUpdateDto.TopicId));
            if (topicEntity == null)
            {
                return new ApiResult<QuizDto>(false, "Topic is null !!!");
            }
        }

        if (quizUpdateDto.GradeId > 0)
        {
            var gradeEntity = await _gradeRepository.GetGradeById(Convert.ToInt32(quizUpdateDto.GradeId));
            if (gradeEntity is null)
            {
                return new ApiResult<QuizDto>(false, "Grade is null");
            }
        }

        var quizMapper = _mapper.Map(quizUpdateDto, quizEntity);
        quizMapper.KeyWord = (quizUpdateDto.TagValues is not null)
            ? quizUpdateDto.TagValues.ConvertToTagString()
            : quizUpdateDto.Title.RemoveDiacritics();
        var quizUpdate = await _quizRepository.UpdateQuiz(quizMapper);
        await CreateOrUpdateElasticQuiz(quizUpdateDto.Id, false);
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

        await _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticQuizzes, id);
        return new ApiResult<bool>(true, "Delete Quiz successfully !!!");
    }

    private async Task CreateOrUpdateElasticQuiz(int id, bool isCreateOrUpdate)
    {
        var quiz = await _quizRepository.GetQuizById(id);
        var result = _mapper.Map<QuizDto>(quiz);
        if (isCreateOrUpdate)
        {
            await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticQuizzes, result, p => p.Id);
        }
        else
        {
            await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticQuizzes, result, result.Id);
        }
    }
}