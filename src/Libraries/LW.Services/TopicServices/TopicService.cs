using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Persistence;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.ProblemRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Data.Repositories.SolutionRepositories;
using LW.Data.Repositories.TagRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Problem;
using LW.Shared.DTOs.Quiz;
using LW.Shared.DTOs.Solution;
using LW.Shared.DTOs.Tag;
using LW.Shared.DTOs.Topic;
using LW.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Serilog;

namespace LW.Services.TopicServices;

public class TopicService : ITopicService
{
    private readonly ITopicRepository _topicRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IProblemRepository _problemRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly ISolutionRepository _solutionRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;
    private readonly IDocumentRepository _documentRepository;
    private readonly IElasticSearchService<TopicDto, int> _elasticSearchService;
    private readonly IElasticSearchService<LessonDto, int> _elasticSearchLessonService;
    private readonly IElasticSearchService<ProblemDto, int> _elasticSearchProblemService;
    private readonly IElasticSearchService<QuizDto, int> _elasticSearchQuizService;
    private readonly IElasticSearchService<SolutionDto, int> _elasticSearchSolutionService;
    private readonly ILogger _logger;

    public TopicService(ITopicRepository topicRepository, IMapper mapper, IDocumentRepository documentRepository,
        IElasticSearchService<TopicDto, int> elasticSearchService, ILogger logger, ILessonRepository lessonRepository,
        IElasticSearchService<LessonDto, int> elasticSearchLessonService, IQuizRepository quizRepository,
        IElasticSearchService<QuizDto, int> elasticSearchQuizService, IProblemRepository problemRepository,
        ISolutionRepository solutionRepository, IElasticSearchService<ProblemDto, int> elasticSearchProblemService,
        IElasticSearchService<SolutionDto, int> elasticSearchSolutionService, ITagRepository tagRepository)
    {
        _topicRepository = topicRepository;
        _mapper = mapper;
        _documentRepository = documentRepository;
        _elasticSearchService = elasticSearchService;
        _logger = logger;
        _lessonRepository = lessonRepository;
        _elasticSearchLessonService = elasticSearchLessonService;
        _quizRepository = quizRepository;
        _elasticSearchQuizService = elasticSearchQuizService;
        _problemRepository = problemRepository;
        _solutionRepository = solutionRepository;
        _elasticSearchProblemService = elasticSearchProblemService;
        _elasticSearchSolutionService = elasticSearchSolutionService;
        _tagRepository = tagRepository;
    }


    public async Task<ApiResult<IEnumerable<TopicDto>>> GetAll(bool? status)
    {
        var list = await _topicRepository.GetAllTopic();
        if (!list.Any())
        {
            return new ApiResult<IEnumerable<TopicDto>>(false, "List topic is null !!!");
        }

        if (status != null)
        {
            list = list.Where(t => t.IsActive == status);
        }

        var result = _mapper.Map<IEnumerable<TopicDto>>(list);
        return new ApiResult<IEnumerable<TopicDto>>(true, result, "Get all topic successfully !");
    }

    public async Task<ApiResult<IEnumerable<TopicDto>>> GetAllTopicByDocument(int id, bool? status)
    {
        var list = await _topicRepository.GetAllTopicByDocument(id);
        if (!list.Any())
        {
            return new ApiResult<IEnumerable<TopicDto>>(false, "List topic is null !!!");
        }
        
        if (status != null)
        {
            list = list.Where(t => t.IsActive == status);
        }

        var result = _mapper.Map<IEnumerable<TopicDto>>(list);
        return new ApiResult<IEnumerable<TopicDto>>(true, result, "Get all topic successfully !");
    }

    public async Task<ApiResult<IEnumerable<TagDto>>> GetTopicIdByTag(int id)
    {
        var topic = await _topicRepository.GetTopicByAllId(id);
        if (topic is null)
        {
            return new ApiResult<IEnumerable<TagDto>>(false, "Topic not found !!!");
        }

        var listStringTag = topic.KeyWord.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries);
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

    public async Task<ApiResult<PagedList<TopicDto>>> GetAllTopicPagination(SearchTopicDto searchTopicDto)
    {
        IEnumerable<TopicDto> topicList;
        if (!string.IsNullOrEmpty(searchTopicDto.Value))
        {
            var topicListSearch = await _elasticSearchService.SearchDocumentFieldAsync(ElasticConstant.ElasticTopics,
                new SearchRequestValue
                {
                    Value = searchTopicDto.Value,
                    Size = searchTopicDto.Size,
                });
            if (topicListSearch is null)
            {
                return new ApiResult<PagedList<TopicDto>>(false, "Topic not found !!!");
            }

            topicList = topicListSearch.ToList();
        }
        else
        {
            var topicListAll = await _topicRepository.GetAllTopic();
            if (!topicListAll.Any())
            {
                return new ApiResult<PagedList<TopicDto>>(false, "Topic is null !!!");
            }

            topicList = _mapper.Map<IEnumerable<TopicDto>>(topicListAll);
        }
        
        if (searchTopicDto.Status != null)
        {
            topicList = topicList.Where(t => t.IsActive == searchTopicDto.Status);
        }

        if (searchTopicDto.DocumentId > 0)
        {
            topicList = topicList.Where(d => d.DocumentId == searchTopicDto.DocumentId).ToList();
        }

        var pagedResult = await PagedList<TopicDto>.ToPageListAsync(topicList.AsQueryable().BuildMock(),
            searchTopicDto.PageIndex, searchTopicDto.PageSize, searchTopicDto.OrderBy, searchTopicDto.IsAscending);
        return new ApiSuccessResult<PagedList<TopicDto>>(pagedResult);
    }

    public async Task<ApiResult<TopicDto>> GetById(int id)
    {
        var obj = await _topicRepository.GetTopicById(id);
        if (obj == null)
        {
            return new ApiResult<TopicDto>(false, "Not found !");
        }

        var result = _mapper.Map<TopicDto>(obj);
        return new ApiResult<TopicDto>(true, result, "Get topic by id successfully !");
    }

    public async Task<ApiResult<bool>> Create(TopicCreateDto model)
    {
        var documentEntity = await _documentRepository.GetDocumentById(model.DocumentId);
        if (documentEntity is null)
        {
            return new ApiResult<bool>(false, "Document of topic not found !!!");
        }

        var topic = _mapper.Map<Topic>(model);
        topic.KeyWord = (model.TagValues is not null)
            ? model.TagValues.ConvertToTagString()
            : model.Title.RemoveDiacritics();
        await _topicRepository.CreateTopic(topic);
        if (model.ParentId != null)
        {
            await CreateOrUpdateElasticTopic(Convert.ToInt32(model.ParentId), true);
        }
        else
        {
            await CreateOrUpdateElasticTopic(topic.Id, true);
        }

        return new ApiResult<bool>(true, "Create topic successfully");
    }

    public async Task<ApiResult<bool>> Update(TopicUpdateDto model)
    {
        var documentEntity = await _documentRepository.GetDocumentById(model.DocumentId);
        if (documentEntity is null)
        {
            return new ApiResult<bool>(false, "Document of topic not found !!!");
        }

        var topicEntity = await _topicRepository.GetTopicById(model.Id);
        if (topicEntity == null)
        {
            return new ApiResult<bool>(false, "Topic is not found !!!");
        }

        var topicUpdate = _mapper.Map(model, topicEntity);
        topicUpdate.KeyWord = (model.TagValues is not null)
            ? model.TagValues.ConvertToTagString()
            : model.Title.RemoveDiacritics();
        await _topicRepository.UpdateTopic(topicUpdate);
        if (model.ParentId is not null)
        {
            await CreateOrUpdateElasticTopic(Convert.ToInt32(model.ParentId), false);
        }
        else
        {
            await CreateOrUpdateElasticTopic(model.Id, false);
        }

        return new ApiResult<bool>(true, "Update topic successfully");
    }

    public async Task<ApiResult<bool>> UpdateStatus(int id)
    {
        var obj = await _topicRepository.GetTopicById(id);
        if (obj == null)
        {
            return new ApiResult<bool>(false, "Not found !");
        }

        obj.IsActive = !obj.IsActive;
        if (obj.ParentId is null)
        {
            await _topicRepository.UpdateTopic(obj);
            var topicChild = await _topicRepository.GetAllTopicChild(obj.Id);
            if (topicChild.Any())
            {
                topicChild.ToList().ForEach(x => x.IsActive = !x.IsActive);
                await _topicRepository.UpdateRangeTopic(topicChild);
            }

            var topic = await _topicRepository.GetTopicById(id);
            var topicResult = _mapper.Map<TopicDto>(topic);
            await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticTopics, topicResult, id);
        }
        else
        {
            await _topicRepository.UpdateTopic(obj);
            var topic = await _topicRepository.GetTopicById(Convert.ToInt32(obj.ParentId));
            var topicResult = _mapper.Map<TopicDto>(topic);
            await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticTopics, topicResult,
                Convert.ToInt32(obj.ParentId));
        }

        return new ApiResult<bool>(true, "Update status of topic successfully");
    }

    public async Task<ApiResult<bool>> Delete(int id)
    {
        var topic = await _topicRepository.GetTopicByAllId(id);
        if (topic == null)
        {
            return new ApiResult<bool>(false, "Not found !");
        }

        // xóa lesson
        var listLesson = await _lessonRepository.GetAllLessonByTopic(topic.Id);
        if (listLesson.Any())
        {
            var listLessonId = listLesson.Select(l => l.Id);
            await _elasticSearchLessonService.DeleteDocumentRangeAsync(ElasticConstant.ElasticLessons,
                listLessonId);
            foreach (var lesson in listLesson)
            {
                var listProblemInLesson = await _problemRepository.GetAllProblemByLesson(lesson.Id);
                foreach (var problem in listProblemInLesson)
                {
                    var listSolutionInLesson = await _solutionRepository.GetAllSolutionByProblemId(problem.Id);
                    if (listSolutionInLesson.Any())
                    {
                        var listSolutionInLessonId = listSolutionInLesson.Select(s => s.Id);
                        await _elasticSearchSolutionService.DeleteDocumentRangeAsync(
                            ElasticConstant.ElasticSolutions, listSolutionInLessonId);
                    }
                }

                var listQuizInLesson = await _quizRepository.GetAllQuizByLessonId(lesson.Id);
                if (listProblemInLesson.Any())
                {
                    var listProblemInLessonId = listProblemInLesson.Select(p => p.Id);
                    await _problemRepository.DeleteRangeProblem(listProblemInLesson);
                    await _elasticSearchProblemService.DeleteDocumentRangeAsync(ElasticConstant.ElasticProblems,
                        listProblemInLessonId);
                }

                if (listQuizInLesson.Any())
                {
                    var listQuizInLessonId = listQuizInLesson.Select(q => q.Id);
                    await _quizRepository.DeleteRangeQuiz(listQuizInLesson);
                    await _elasticSearchQuizService.DeleteDocumentRangeAsync(ElasticConstant.ElasticQuizzes,
                        listQuizInLessonId);
                }
            }
        }

        var listProblem = await _problemRepository.GetAllProblemByTopic(id);
        foreach (var problem in listProblem)
        {
            var listSolution = await _solutionRepository.GetAllSolutionByProblemId(problem.Id);
            if (listSolution.Any())
            {
                var listSolutionInLessonId = listSolution.Select(s => s.Id);
                await _elasticSearchSolutionService.DeleteDocumentRangeAsync(ElasticConstant.ElasticSolutions,
                    listSolutionInLessonId);
            }
        }

        var listQuiz = await _quizRepository.GetAllQuizByTopicId(id);
        if (listQuiz.Any())
        {
            var listQuizId = listQuiz.Select(q => q.Id);
            await _quizRepository.DeleteRangeQuiz(listQuiz);
            await _elasticSearchQuizService.DeleteDocumentRangeAsync(ElasticConstant.ElasticQuizzes, listQuizId);
        }

        if (listProblem.Any())
        {
            var listProblemId = listProblem.Select(p => p.Id);
            await _problemRepository.DeleteRangeProblem(listProblem);
            await _elasticSearchProblemService.DeleteDocumentRangeAsync(ElasticConstant.ElasticProblems, listProblemId);
        }

        if (topic.ParentId is null)
        {
            var topicDelete = await _topicRepository.GetTopicById(id);
            var topicResult = _mapper.Map<TopicDto>(topicDelete);
            await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticTopics, topicResult, id);
        }
        else
        {
            var topicChildDelete = await _topicRepository.GetTopicById(Convert.ToInt32(topic.ParentId));
            var topicResult = _mapper.Map<TopicDto>(topicChildDelete);
            await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticTopics, topicResult,
                Convert.ToInt32(topic.ParentId));
        }

        var isDeleted = await _topicRepository.DeleteTopic(id);
        if (!isDeleted)
        {
            return new ApiResult<bool>(false, "Delete topic failed");
        }

        return new ApiResult<bool>(true, "Delete topic successfully");
    }

    public async Task CreateOrUpdateElasticTopic(int id, bool isCreateOrUpdate)
    {
        var topic = await _topicRepository.GetTopicById(id);
        var result = _mapper.Map<TopicDto>(topic);
        if (isCreateOrUpdate)
        {
            await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticTopics, result, t => t.Id);
        }
        else
        {
            await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticDocuments, result, result.Id);
            var quizzes = await _quizRepository.GetAllQuizByTopicId(id, true);
            var lessons = await _lessonRepository.GetAllLessonByTopic(id);
            var resultQuiz = _mapper.Map<IEnumerable<QuizDto>>(quizzes);
            var resultLesson = _mapper.Map<IEnumerable<LessonDto>>(lessons);
            await _elasticSearchLessonService.UpdateDocumentRangeAsync(ElasticConstant.ElasticLessons, resultLesson,
                d => d.Id);
            await _elasticSearchQuizService.UpdateDocumentRangeAsync(ElasticConstant.ElasticQuizzes, resultQuiz,
                d => d.Id);
        }
    }
}