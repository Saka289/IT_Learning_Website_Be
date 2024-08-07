using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.ProblemRepositories;
using LW.Data.Repositories.SubmissionRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs.Problem;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using MockQueryable.Moq;

namespace LW.Services.ProblemServices;

public class ProblemService : IProblemService
{
    private readonly IProblemRepository _problemRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IMapper _mapper;
    private readonly IElasticSearchService<ProblemDto, int> _elasticSearchService;

    public ProblemService(IProblemRepository problemRepository, IMapper mapper,
        IElasticSearchService<ProblemDto, int> elasticSearchService, ITopicRepository topicRepository,
        ILessonRepository lessonRepository, ISubmissionRepository submissionRepository)
    {
        _problemRepository = problemRepository;
        _mapper = mapper;
        _elasticSearchService = elasticSearchService;
        _topicRepository = topicRepository;
        _lessonRepository = lessonRepository;
        _submissionRepository = submissionRepository;
    }

    public async Task<ApiResult<IEnumerable<ProblemDto>>> GetAllProblem()
    {
        var problem = await _problemRepository.GetAllProblem();
        if (!problem.Any())
        {
            return new ApiResult<IEnumerable<ProblemDto>>(false, "Problem not found !!!");
        }

        var result = _mapper.Map<IEnumerable<ProblemDto>>(problem);
        return new ApiSuccessResult<IEnumerable<ProblemDto>>(result);
    }

    public async Task<ApiResult<PagedList<ProblemDto>>> GetAllProblemPagination(SearchProblemDto searchProblemDto)
    {
        IEnumerable<ProblemDto> problemList;
        if (!string.IsNullOrEmpty(searchProblemDto.Value))
        {
            var problemListSearch = await _elasticSearchService.SearchDocumentFieldAsync(
                ElasticConstant.ElasticProblems, new SearchRequestValue
                {
                    Value = searchProblemDto.Value,
                    Size = searchProblemDto.Size,
                });
            if (problemListSearch is null)
            {
                return new ApiResult<PagedList<ProblemDto>>(false, "Problem not found !!!");
            }

            problemList = problemListSearch.ToList();
        }
        else
        {
            var problemListAll = await _problemRepository.GetAllProblem();
            if (!problemListAll.Any())
            {
                return new ApiResult<PagedList<ProblemDto>>(false, "Problem not found !!!");
            }

            problemList = _mapper.Map<IEnumerable<ProblemDto>>(problemListAll);
        }

        foreach (var item in problemList)
        {
            var status = await _submissionRepository.GetAllSubmissionByStatus(searchProblemDto.UserId, item.Id);
            item.Status = status ? EStatusProblem.Solved : EStatusProblem.ToDo;
        }
        
        if (searchProblemDto.Difficulty > 0)
        {
            problemList = problemList.Where(p => p.Difficulty == (int)searchProblemDto.Difficulty);
        }

        if (searchProblemDto.TopicId > 0)
        {
            problemList = problemList.Where(p => p.TopicId == searchProblemDto.TopicId);
        }

        if (searchProblemDto.LessonId > 0)
        {
            problemList = problemList.Where(p => p.LessonId == searchProblemDto.LessonId);
        }

        if (searchProblemDto.Status > 0)
        {
            problemList = problemList.Where(p => p.Status == searchProblemDto.Status);
        }

        var pagedResult = await PagedList<ProblemDto>.ToPageListAsync(problemList.AsQueryable().BuildMock(),
            searchProblemDto.PageIndex, searchProblemDto.PageSize, searchProblemDto.OrderBy,
            searchProblemDto.IsAscending);
        return new ApiSuccessResult<PagedList<ProblemDto>>(pagedResult);
    }

    public async Task<ApiResult<ProblemDto>> GetProblemById(int id)
    {
        var problem = await _problemRepository.GetProblemById(id);
        if (problem is null)
        {
            return new ApiResult<ProblemDto>(false, "Problem not found !!!");
        }

        var result = _mapper.Map<ProblemDto>(problem);
        return new ApiSuccessResult<ProblemDto>(result);
    }

    public async Task<ApiResult<ProblemDto>> CreateProblem(ProblemCreateDto problemCreateDto)
    {
        if (problemCreateDto.TopicId > 0)
        {
            var topic = await _topicRepository.GetTopicByAllId(Convert.ToInt32(problemCreateDto.TopicId));
            if (topic is null)
            {
                return new ApiResult<ProblemDto>(false, "Topic not found !!!");
            }
        }

        if (problemCreateDto.LessonId > 0)
        {
            var lesson = await _lessonRepository.GetLessonById(Convert.ToInt32(problemCreateDto.LessonId));
            if (lesson is null)
            {
                return new ApiResult<ProblemDto>(false, "Lesson not found !!!");
            }
        }

        var problemEntity = _mapper.Map<Problem>(problemCreateDto);
        problemEntity.KeyWord = problemCreateDto.Title.RemoveDiacritics();
        var problemCreate = await _problemRepository.CreateProblem(problemEntity);
        var result = _mapper.Map<ProblemDto>(problemCreate);
        _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticProblems, result, p => p.Id);
        return new ApiSuccessResult<ProblemDto>(result);
    }

    public async Task<ApiResult<ProblemDto>> UpdateProblem(ProblemUpdateDto problemUpdateDto)
    {
        var problem = await _problemRepository.GetProblemById(problemUpdateDto.Id);
        if (problem is null)
        {
            return new ApiResult<ProblemDto>(false, "Problem not found !!!");
        }

        if (problemUpdateDto.TopicId > 0)
        {
            var topic = await _topicRepository.GetTopicByAllId(Convert.ToInt32(problemUpdateDto.TopicId));
            if (topic is null)
            {
                return new ApiResult<ProblemDto>(false, "Topic not found !!!");
            }
        }

        if (problemUpdateDto.LessonId > 0)
        {
            var lesson = await _lessonRepository.GetLessonById(Convert.ToInt32(problemUpdateDto.LessonId));
            if (lesson is null)
            {
                return new ApiResult<ProblemDto>(false, "Lesson not found !!!");
            }
        }

        var problemMapper = _mapper.Map(problemUpdateDto, problem);
        problemMapper.KeyWord = problemUpdateDto.Title.RemoveDiacritics();
        var problemUpdate = await _problemRepository.UpdateProblem(problemMapper);
        var result = _mapper.Map<ProblemDto>(problemUpdate);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticProblems, result, problemUpdateDto.Id);
        return new ApiSuccessResult<ProblemDto>(result);
    }

    public async Task<ApiResult<bool>> UpdateStatusProblem(int id)
    {
        var problem = await _problemRepository.GetProblemById(id);
        if (problem is null)
        {
            return new ApiResult<bool>(false, "Problem not found !!!");
        }

        problem.IsActive = !problem.IsActive;
        var problemUpdate = await _problemRepository.UpdateProblem(problem);
        var result = _mapper.Map<ProblemDto>(problemUpdate);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticProblems, result, id);
        return new ApiSuccessResult<bool>(true);
    }

    public async Task<ApiResult<bool>> DeleteProblem(int id)
    {
        var problem = await _problemRepository.GetProblemById(id);
        if (problem is null)
        {
            return new ApiResult<bool>(false, "Problem not found !!!");
        }

        var problemDelete = await _problemRepository.DeleteProblem(id);
        if (!problemDelete)
        {
            return new ApiResult<bool>(false, "Failed to delete problem !!!");
        }

        _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticProblems, id);
        return new ApiSuccessResult<bool>(problemDelete);
    }
}