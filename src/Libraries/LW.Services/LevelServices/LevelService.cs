using System.Text;
using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.GradeRepositories;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.LevelRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Grade;
using LW.Shared.DTOs.Lesson;
using Serilog;
using LW.Shared.DTOs.Level;
using LW.Shared.DTOs.Topic;
using LW.Shared.SeedWork;
using MockQueryable.Moq;

namespace LW.Services.LevelServices;

public class LevelService : ILevelService
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly ILevelRepository _levelRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly ILessonRepository _lessonRepository;


    private readonly IElasticSearchService<LevelDto, int> _elasticSearchService;
    private readonly IElasticSearchService<GradeDto, int> _elasticSearchGradeService;
    private readonly IElasticSearchService<DocumentDto, int> _elasticSearchDocumentService;
    private readonly IElasticSearchService<TopicDto, int> _elasticSearchTopicService;
    private readonly IElasticSearchService<LessonDto, int> _elasticSearchLessonService;


    public LevelService(ILogger logger, ILevelRepository levelRepository, IMapper mapper,
        IElasticSearchService<LevelDto, int> elasticSearchService, IGradeRepository gradeRepository,
        IDocumentRepository documentRepository, ITopicRepository topicRepository, ILessonRepository lessonRepository,
        IElasticSearchService<GradeDto, int> elasticSearchGradeService,
        IElasticSearchService<DocumentDto, int> elasticSearchDocumentService,
        IElasticSearchService<TopicDto, int> elasticSearchTopicService,
        IElasticSearchService<LessonDto, int> elasticSearchLessonService)
    {
        _logger = logger;
        _levelRepository = levelRepository;
        _mapper = mapper;
        _elasticSearchService = elasticSearchService;
        _gradeRepository = gradeRepository;
        _documentRepository = documentRepository;
        _topicRepository = topicRepository;
        _lessonRepository = lessonRepository;
        _elasticSearchGradeService = elasticSearchGradeService;
        _elasticSearchDocumentService = elasticSearchDocumentService;
        _elasticSearchTopicService = elasticSearchTopicService;
        _elasticSearchLessonService = elasticSearchLessonService;
    }

    public async Task<ApiResult<bool>> Create(LevelDtoForCreate model)
    {
        var level = _mapper.Map<Level>(model);
        level.KeyWord = model.Title.RemoveDiacritics();
        await _levelRepository.CreateLevel(level);

        var result = _mapper.Map<LevelDto>(level);
        _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticLevels, result, g => g.Id);
        return new ApiResult<bool>(true, "Create level successfully");
    }


    public async Task<ApiResult<bool>> Update(LevelDtoForUpdate model)
    {
        var levelInDb = await _levelRepository.GetLevelById(model.Id);
        if (levelInDb == null)
        {
            return new ApiResult<bool>(false, "Not found");
        }

        var obj = _mapper.Map(model, levelInDb);
        // Sau khi ánh xạ, levelIbDb sẽ có các giá trị từ model:
        obj.KeyWord = model.Title.RemoveDiacritics();
        await _levelRepository.UpdateLevel(obj);
        var result = _mapper.Map<LevelDto>(obj);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticLevels, result, model.Id);
        return new ApiResult<bool>(true, "Update level successfully");
    }

    public async Task<ApiResult<bool>> UpdateStatus(int id)
    {
        var objLevel = await _levelRepository.GetLevelById(id);
        if (objLevel == null)
        {
            return new ApiResult<bool>(false, "Not found !");
        }

        objLevel.IsActive = !objLevel.IsActive;
        await _levelRepository.UpdateLevel(objLevel);
        var result = _mapper.Map<LevelDto>(objLevel);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticLevels, result, id);
        return new ApiResult<bool>(true, "Update Status of level successfully");
    }

    public async Task<ApiResult<bool>> Delete(int id)
    {
        var level = await _levelRepository.GetLevelById(id);
        if (level == null)
        {
            return new ApiResult<bool>(false, "Not found !");
        }

        //delete grade -document- topic -lesson
        var grades = await _gradeRepository.GetAllGradeByLevel(id);
        if (grades.Any())
        {
            var listgradeId = grades.Select(x => x.Id).ToList();
            await _elasticSearchGradeService.DeleteDocumentRangeAsync(ElasticConstant.ElasticGrades, listgradeId);
            foreach (var grade in grades)
            {
                var documents = await _documentRepository.GetAllDocumentByGrade(grade.Id);
                if (documents.Any())
                {
                    var listdocumentId = documents.Select(x => x.Id).ToList();
                    await _elasticSearchDocumentService.DeleteDocumentRangeAsync(ElasticConstant.ElasticDocuments,
                        listdocumentId);
                    foreach (var doc in documents)
                    {
                        var listTopic = await _topicRepository.GetAllTopicByDocument(id);
                        if (listTopic.Any())
                        {
                            var listTopicId = listTopic.Select(x => x.Id).ToList();
                            await _elasticSearchTopicService.DeleteDocumentRangeAsync(ElasticConstant.ElasticTopics,
                                listTopicId);
                            foreach (var topic in listTopic)
                            {
                                var topicChilds = await _topicRepository.GetAllTopicChildByParentId(topic.Id);
                                if (topicChilds.Any())
                                {
                                    foreach (var topicChild in topicChilds)
                                    {
                                        var listLesson = await _lessonRepository.GetAllLessonByTopic(topicChild.Id);
                                        if (listLesson.Any())
                                        {
                                            var listLessonDto = _mapper.Map<IEnumerable<LessonDto>>(listLesson);
                                            var listId = listLessonDto.Select(x => x.Id).ToList();
                                            await _elasticSearchLessonService.DeleteDocumentRangeAsync(
                                                ElasticConstant.ElasticLessons, listId);
                                        }
                                    }
                                }
                                else
                                {
                                    var listLesson = await _lessonRepository.GetAllLessonByTopic(topic.Id);
                                    if (listLesson.Any())
                                    {
                                        var listLessonDto = _mapper.Map<IEnumerable<LessonDto>>(listLesson);
                                        var listId = listLessonDto.Select(x => x.Id).ToList();
                                        await _elasticSearchLessonService.DeleteDocumentRangeAsync(
                                            ElasticConstant.ElasticLessons, listId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        var isDeleted = await _levelRepository.DeleteLevel(id);
        if (!isDeleted)
        {
            return new ApiResult<bool>(false, "Delete level failed");
        }

        _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticLevels, id);
        return new ApiResult<bool>(true, "Delete level successfully");
    }

    public async Task<ApiResult<IEnumerable<LevelDto>>> GetAll()
    {
        var list = await _levelRepository.GetAllLevel();
        if (list == null)
        {
            return new ApiResult<IEnumerable<LevelDto>>(false, "Levels is null !!!");
        }

        var result = _mapper.Map<IEnumerable<LevelDto>>(list);
        return new ApiResult<IEnumerable<LevelDto>>(true, result, "Get all level successfully");
    }

    public async Task<ApiResult<LevelDto>> GetById(int id)
    {
        var level = await _levelRepository.GetLevelById(id);
        if (level == null)
        {
            return new ApiResult<LevelDto>(false, "Not Found");
        }

        var result = _mapper.Map<LevelDto>(level);
        return new ApiResult<LevelDto>(true, result, "Get level successfully");
    }

    public async Task<ApiResult<PagedList<LevelDto>>> SearchByLevelPagination(SearchLevelDto searchLevelDto)
    {
        var levels = await _elasticSearchService.SearchDocumentAsync(ElasticConstant.ElasticLevels, searchLevelDto);
        if (levels is null)
        {
            return new ApiResult<PagedList<LevelDto>>(false, $"Levels not found by {searchLevelDto.Key} !!!");
        }

        var result = _mapper.Map<IEnumerable<LevelDto>>(levels);
        var pagedResult = await PagedList<LevelDto>.ToPageListAsync(result.AsQueryable().BuildMock(),
            searchLevelDto.PageIndex, searchLevelDto.PageSize, searchLevelDto.OrderBy, searchLevelDto.IsAscending);
        return new ApiSuccessResult<PagedList<LevelDto>>(pagedResult);
    }

    public async Task<ApiResult<PagedList<LevelDto>>> GetAllLevelPagination(
        PagingRequestParameters pagingRequestParameters)
    {
        var levels = await _levelRepository.GetAllLevelPagination();
        if (levels == null)
        {
            return new ApiResult<PagedList<LevelDto>>(false, "Levels is null !!!");
        }

        var result = _mapper.ProjectTo<LevelDto>(levels);
        var pagedResult = await PagedList<LevelDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);
        return new ApiSuccessResult<PagedList<LevelDto>>(pagedResult);
    }

    public Task<ApiResult<bool>> DeleteRangeLevel(IEnumerable<int> ids)
    {
        throw new NotImplementedException();
    }
}