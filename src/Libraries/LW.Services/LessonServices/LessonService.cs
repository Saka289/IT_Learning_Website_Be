using System.Text;
using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.ProblemRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Data.Repositories.SolutionRepositories;
using LW.Data.Repositories.TagRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs.File;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Problem;
using LW.Shared.DTOs.Quiz;
using LW.Shared.DTOs.Solution;
using LW.Shared.DTOs.Tag;
using LW.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Serilog;

namespace LW.Services.LessonServices;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IProblemRepository _problemRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly ISolutionRepository _solutionRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IElasticSearchService<LessonDto, int> _elasticSearchService;
    private readonly IElasticSearchService<ProblemDto, int> _elasticSearchProblemService;
    private readonly IElasticSearchService<QuizDto, int> _elasticSearchQuizService;
    private readonly IElasticSearchService<SolutionDto, int> _elasticSearchSolutionService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public LessonService(ILessonRepository lessonRepository, ITopicRepository topicRepository, IMapper mapper,
        IElasticSearchService<LessonDto, int> elasticSearchService, ICloudinaryService cloudinaryService,
        ILogger logger, IProblemRepository problemRepository, IQuizRepository quizRepository,
        ISolutionRepository solutionRepository, IElasticSearchService<ProblemDto, int> elasticSearchProblemService,
        IElasticSearchService<QuizDto, int> elasticSearchQuizService,
        IElasticSearchService<SolutionDto, int> elasticSearchSolutionService, ITagRepository tagRepository)
    {
        _lessonRepository = lessonRepository;
        _topicRepository = topicRepository;
        _mapper = mapper;
        _elasticSearchService = elasticSearchService;
        _cloudinaryService = cloudinaryService;
        _logger = logger;
        _problemRepository = problemRepository;
        _quizRepository = quizRepository;
        _solutionRepository = solutionRepository;
        _elasticSearchProblemService = elasticSearchProblemService;
        _elasticSearchQuizService = elasticSearchQuizService;
        _elasticSearchSolutionService = elasticSearchSolutionService;
        _tagRepository = tagRepository;
    }

    public async Task<ApiResult<IEnumerable<LessonDto>>> GetAllLesson()
    {
        var lessonList = await _lessonRepository.GetAllLesson();
        if (!lessonList.Any())
        {
            return new ApiResult<IEnumerable<LessonDto>>(false, "Lesson is null !!!");
        }

        var result = _mapper.Map<IEnumerable<LessonDto>>(lessonList);
        return new ApiSuccessResult<IEnumerable<LessonDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<LessonDto>>> GetAllLessonByTopic(int id)
    {
        var lessonList = await _lessonRepository.GetAllLessonByTopic(id);
        if (!lessonList.Any())
        {
            return new ApiResult<IEnumerable<LessonDto>>(false, "Lesson is null !!!");
        }

        var result = _mapper.Map<IEnumerable<LessonDto>>(lessonList);
        return new ApiSuccessResult<IEnumerable<LessonDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<TagDto>>> GetLessonIdByTag(int id)
    {
        var lesson = await _lessonRepository.GetLessonById(id);
        if (lesson is null)
        {
            return new ApiResult<IEnumerable<TagDto>>(false, "Lesson not found !!!");
        }

        var listStringTag = lesson.KeyWord.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries);
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

    public async Task<ApiResult<PagedList<LessonDto>>> GetAllLessonPagination(SearchLessonDto searchLessonDto)
    {
        IEnumerable<LessonDto> lessonList;
        if (!string.IsNullOrEmpty(searchLessonDto.Value))
        {
            var lessonListSearch = await _elasticSearchService.SearchDocumentFieldAsync(ElasticConstant.ElasticLessons,
                new SearchRequestValue
                {
                    Value = searchLessonDto.Value,
                    Size = searchLessonDto.Size,
                });
            if (lessonListSearch is null)
            {
                return new ApiResult<PagedList<LessonDto>>(false, "Lesson not found !!!");
            }

            lessonList = lessonListSearch.ToList();
        }
        else
        {
            var lessonListAll = await _lessonRepository.GetAllLesson();
            if (!lessonListAll.Any())
            {
                return new ApiResult<PagedList<LessonDto>>(false, "Lesson is null !!!");
            }

            lessonList = _mapper.Map<IEnumerable<LessonDto>>(lessonListAll);
        }

        if (searchLessonDto.TopicId > 0)
        {
            lessonList = lessonList.Where(t => t.TopicId == searchLessonDto.TopicId).ToList();
        }

        var pagedResult = await PagedList<LessonDto>.ToPageListAsync(lessonList.AsQueryable().BuildMock(),
            searchLessonDto.PageIndex, searchLessonDto.PageSize, searchLessonDto.OrderBy, searchLessonDto.IsAscending);
        return new ApiSuccessResult<PagedList<LessonDto>>(pagedResult);
    }

    public async Task<ApiResult<LessonDto>> GetLessonById(int id)
    {
        var lessonEntity = await _lessonRepository.GetLessonById(id);
        if (lessonEntity == null)
        {
            return new ApiResult<LessonDto>(false, "Lesson is null !!!");
        }

        var topicEntity = await _topicRepository.GetTopicById(lessonEntity.TopicId);
        if (topicEntity is not null)
        {
            lessonEntity.Topic = topicEntity;
        }

        var result = _mapper.Map<LessonDto>(lessonEntity);
        return new ApiSuccessResult<LessonDto>(result);
    }

    public async Task<ApiResult<LessonDto>> CreateLesson(LessonCreateDto lessonCreateDto)
    {
        var topicEntity = await _topicRepository.GetTopicById(lessonCreateDto.TopicId);
        if (topicEntity is null)
        {
            return new ApiResult<LessonDto>(false, "TopicId not found !!!");
        }

        var isDuplicateIndex = await FindDuplicateLessonIndex(lessonCreateDto.Index);
        if (isDuplicateIndex)
        {
            return new ApiResult<LessonDto>(false, "Lesson Index is Duplicate !!!");
        }

        var isDuplicateLesson = await FindDuplicateLesson(lessonCreateDto.Title!);
        if (isDuplicateLesson)
        {
            return new ApiResult<LessonDto>(false, "Lesson Title is Duplicate !!!");
        }

        var lessonEntity = _mapper.Map<Lesson>(lessonCreateDto);
        var filePath = new FileDto();
        if (string.IsNullOrEmpty(lessonEntity.Content) || lessonEntity.Content == null)
        {
            var htmlContent = await _cloudinaryService.ConvertPdfToHtml(lessonCreateDto.FilePath);
            lessonEntity.Content = htmlContent.Base64Encode();
            filePath = await _cloudinaryService.CreateFileAsync(lessonCreateDto.FilePath,
                CloudinaryConstant.FolderLessonFile);
        }
        else
        {
            filePath = await _cloudinaryService.ConvertHtmlToPdf(lessonCreateDto.Content, lessonCreateDto.Title,
                CloudinaryConstant.FolderLessonFile);
            lessonEntity.Content = lessonCreateDto.Content.Base64Encode();
        }

        lessonEntity.KeyWord = (lessonCreateDto.TagValues is not null)
            ? lessonCreateDto.TagValues.ConvertToTagString()
            : lessonCreateDto.Title.RemoveDiacritics();
        lessonEntity.FilePath = filePath.Url;
        lessonEntity.PublicId = filePath.PublicId;
        lessonEntity.UrlDownload = filePath.UrlDownload;
        await _lessonRepository.CreateLesson(lessonEntity);
        await _lessonRepository.SaveChangesAsync();
        await CreateOrUpdateElasticLesson(lessonEntity.Id, true);
        var result = _mapper.Map<LessonDto>(lessonEntity);
        return new ApiSuccessResult<LessonDto>(result);
    }

    public async Task<ApiResult<LessonDto>> UpdateLesson(LessonUpdateDto lessonUpdateDto)
    {
        var topicEntity = await _topicRepository.GetTopicById(lessonUpdateDto.TopicId);
        if (topicEntity is null)
        {
            return new ApiResult<LessonDto>(false, "TopicId not found !!!");
        }

        var lessonEntity = await _lessonRepository.GetLessonById(lessonUpdateDto.Id);
        if (lessonEntity is null)
        {
            return new ApiResult<LessonDto>(false, "Lesson not found !!!");
        }
        
        var isDuplicateIndex = await FindDuplicateLessonIndex(lessonUpdateDto.Index, lessonUpdateDto.Id);
        if (isDuplicateIndex)
        {
            return new ApiResult<LessonDto>(false, "Lesson Index is Duplicate !!!");
        }
        
        var isDuplicate = await FindDuplicateLesson(lessonUpdateDto.Title!, lessonUpdateDto.Id);
        if (isDuplicate)
        {
            return new ApiResult<LessonDto>(false, "Lesson Title is Duplicate !!!");
        }

        var model = _mapper.Map(lessonUpdateDto, lessonEntity);
        var filePath = new FileDto();
        var lessonEntityUpdate = await _lessonRepository.GetLessonById(lessonUpdateDto.Id);
        if (lessonUpdateDto.FilePath != null && lessonUpdateDto.FilePath.Length > 0)
        {
            var htmlContent = await _cloudinaryService.ConvertPdfToHtml(lessonUpdateDto.FilePath);
            lessonEntity.Content = htmlContent.Base64Encode();
            filePath = await _cloudinaryService.UpdateFileAsync(lessonEntityUpdate.PublicId, lessonUpdateDto.FilePath);
        }
        else
        {
            await _cloudinaryService.DeleteFileAsync(lessonEntityUpdate.PublicId);
            filePath = await _cloudinaryService.ConvertHtmlToPdf(lessonUpdateDto.Content, lessonUpdateDto.Title,
                CloudinaryConstant.FolderLessonFile);
            lessonEntity.Content = lessonUpdateDto.Content.Base64Encode();
        }

        model.FilePath = filePath.Url;
        model.PublicId = filePath.PublicId;
        model.UrlDownload = filePath.UrlDownload;
        model.KeyWord = (lessonUpdateDto.TagValues is not null) ? lessonUpdateDto.TagValues.ConvertToTagString() : lessonUpdateDto.Title.RemoveDiacritics();
        var updateLesson = await _lessonRepository.UpdateLesson(model);
        await _lessonRepository.SaveChangesAsync();
        await CreateOrUpdateElasticLesson(lessonUpdateDto.Id, true);
        var result = _mapper.Map<LessonDto>(updateLesson);
        return new ApiSuccessResult<LessonDto>(result);
    }

    public async Task<ApiResult<bool>> UpdateLessonStatus(int id)
    {
        var lessonEntity = await _lessonRepository.GetLessonById(id);
        if (lessonEntity is null)
        {
            return new ApiResult<bool>(false, "Lesson not found !!!");
        }

        lessonEntity.IsActive = !lessonEntity.IsActive;
        await _lessonRepository.UpdateLesson(lessonEntity);
        await _lessonRepository.SaveChangesAsync();
        var result = _mapper.Map<LessonDto>(lessonEntity);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticLessons, result, id);
        return new ApiSuccessResult<bool>(true, "Lesson update successfully !!!");
    }

    public async Task<ApiResult<bool>> DeleteLesson(int id)
    {
        var lessonEntity = await _lessonRepository.GetLessonById(id);
        if (lessonEntity is null)
        {
            return new ApiResult<bool>(false, "Lesson not found !!!");
        }

        // delete problem quiz
        var listProblemInLesson = await _problemRepository.GetAllProblemByLesson(id);
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

        var listQuizInLesson = await _quizRepository.GetAllQuizByLessonId(id);
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


        var lessonDelete = await _lessonRepository.DeleteLesson(id);
        if (!lessonDelete)
        {
            return new ApiResult<bool>(false, "Failed Delete Lesson not found !!!");
        }

        await _cloudinaryService.DeleteFileAsync(lessonEntity.PublicId);
        await _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticLessons, id);
        return new ApiSuccessResult<bool>(true, "Delete Lesson Successfully !!!");
    }

    public async Task<ApiResult<bool>> DeleteRangeLesson(IEnumerable<int> ids)
    {
        var listId = new List<Lesson>();
        foreach (var itemId in ids)
        {
            var lessonEntity = await _lessonRepository.GetLessonById(itemId);
            if (lessonEntity is null)
            {
                _logger.Information($"Lesson not found with id {itemId} !!!");
            }
            else
            {
                listId.Add(lessonEntity);
            }
        }

        foreach (var itemLesson in listId.Select(l => l.Id))
        {
            var listProblemInLesson = await _problemRepository.GetAllProblemByLesson(itemLesson);
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

            var listQuizInLesson = await _quizRepository.GetAllQuizByLessonId(itemLesson);
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

        var lesson = await _lessonRepository.DeleteRangeLesson(ids);
        if (!lesson)
        {
            return new ApiResult<bool>(false, "Failed Delete Lesson not found !!!");
        }

        await _elasticSearchService.DeleteDocumentRangeAsync(ElasticConstant.ElasticLessons, listId.Select(x => x.Id));
        return new ApiSuccessResult<bool>(true, "Delete Lessons Successfully !!!");
    }

    private async Task CreateOrUpdateElasticLesson(int id, bool isCreateOrUpdate)
    {
        var lesson = await _lessonRepository.GetLessonById(id);
        var result = _mapper.Map<LessonDto>(lesson);
        if (isCreateOrUpdate)
        {
            await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticLessons, result, l => l.Id);
        }
        else
        {
            await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticLessons, result, result.Id);
            var quizzes = await _quizRepository.GetAllQuizByTopicId(id, true);
            var resultQuiz = _mapper.Map<IEnumerable<QuizDto>>(quizzes);
            await _elasticSearchQuizService.UpdateDocumentRangeAsync(ElasticConstant.ElasticQuizzes, resultQuiz,
                d => d.Id);
        }
    }

    private async Task<bool> FindDuplicateLesson(string title, int id = 0)
    {
        var listLesson = await _lessonRepository.GetAllLesson();
        if (id > 0)
        {
            listLesson = listLesson.Where(l => l.Id != id);
        }

        if (listLesson.Any(l => l.Title!.Trim().ToLower().Equals(title.Trim().ToLower())))
        {
            return true;
        }

        return false;
    }

    private async Task<bool> FindDuplicateLessonIndex(int index, int id = 0)
    {
        var listLesson = await _lessonRepository.GetAllLesson();
        if (id > 0)
        {
            listLesson = listLesson.Where(l => l.Id != id);
        }
        
        if (listLesson.Any(l => l.Index == index))
        {
            return true;
        }

        return false;
    }
}