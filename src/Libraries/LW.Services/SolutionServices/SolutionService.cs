using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.ProblemRepositories;
using LW.Data.Repositories.SolutionRepositories;
using LW.Infrastructure.Extensions;
using LW.Services.UserServices;
using LW.Shared.Constant;
using LW.Shared.SeedWork;
using LW.Shared.Solution;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;

namespace LW.Services.SolutionServices;

public class SolutionService : ISolutionService
{
    private readonly ISolutionRepository _solutionRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IProblemRepository _problemRepository;
    private readonly IMapper _mapper;
    private readonly IElasticSearchService<SolutionDto, int> _elasticSearchService;

    public SolutionService(ISolutionRepository solutionRepository, IMapper mapper,
        IElasticSearchService<SolutionDto, int> elasticSearchService, UserManager<ApplicationUser> userManager, IProblemRepository problemRepository)
    {
        _solutionRepository = solutionRepository;
        _mapper = mapper;
        _elasticSearchService = elasticSearchService;
        _userManager = userManager;
        _problemRepository = problemRepository;
    }

    public async Task<ApiResult<IEnumerable<SolutionDto>>> GetAllSolutionByProblemId(int problemId)
    {
        var solutionList = await _solutionRepository.GetAllSolutionByProblemId(problemId, true);
        if (!solutionList.Any())
        {
            return new ApiResult<IEnumerable<SolutionDto>>(false, "Solution not found !!!");
        }

        var result = _mapper.Map<IEnumerable<SolutionDto>>(solutionList);
        return new ApiSuccessResult<IEnumerable<SolutionDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<SolutionDto>>> SearchSolutionByProblemId(int problemId,
        SearchRequestValue searchRequestValue)
    {
        var solutionList = await _elasticSearchService.SearchAllDocumentFieldAsync(ElasticConstant.ElasticSolutions, searchRequestValue);
        if (!solutionList.Any())
        {
            return new ApiResult<IEnumerable<SolutionDto>>(false, "Solution not found !!!");
        }

        if (problemId > 0)
        {
            solutionList = solutionList.Where(p => p.ProblemId == problemId).ToList();
        }

        var result = _mapper.Map<IEnumerable<SolutionDto>>(solutionList);
        return new ApiSuccessResult<IEnumerable<SolutionDto>>(result);
    }

    public async Task<ApiResult<SolutionDto>> GetSolutionById(int id)
    {
        var solution = await _solutionRepository.GetSolutionById(id);
        if (solution is null)
        {
            return new ApiResult<SolutionDto>(false, "Solution not found !!!");
        }

        var result = _mapper.Map<SolutionDto>(solution);
        return new ApiSuccessResult<SolutionDto>(result);
    }

    public async Task<ApiResult<SolutionDto>> CreateSolution(SolutionCreateDto solutionCreateDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(solutionCreateDto.UserId));
        if (user is null)
        {
            return new ApiResult<SolutionDto>(false, "User not found !!!");
        }

        var problem = await _problemRepository.GetProblemById(solutionCreateDto.ProblemId);
        if (problem is null)
        {
            return new ApiResult<SolutionDto>(false, "Problem not found !!!");
        }

        var solutionMapper = _mapper.Map<Solution>(solutionCreateDto);
        solutionMapper.Coding = solutionCreateDto.Coding.Base64Encode();
        var solutionCreate = await _solutionRepository.CreateSolution(solutionMapper);
        solutionCreate.ApplicationUser = user;
        // solutionCreate.Problem = problem;
        var result = _mapper.Map<SolutionDto>(solutionCreate);
        _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticSolutions, result, s => s.Id);
        return new ApiSuccessResult<SolutionDto>(result);
    }

    public async Task<ApiResult<SolutionDto>> UpdateSolution(SolutionUpdateDto solutionUpdateDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(solutionUpdateDto.UserId));
        if (user is null)
        {
            return new ApiResult<SolutionDto>(false, "User not found !!!");
        }

        var problem = await _problemRepository.GetProblemById(solutionUpdateDto.ProblemId);
        if (problem is null)
        {
            return new ApiResult<SolutionDto>(false, "Problem not found !!!");
        }

        var solutionEntity = await _solutionRepository.GetSolutionById(solutionUpdateDto.Id);
        if (solutionEntity is null)
        {
            return new ApiResult<SolutionDto>(false, "Solution not found !!!");
        }

        var solutionMapper = _mapper.Map(solutionUpdateDto, solutionEntity);
        solutionMapper.Coding = solutionMapper.Coding.Base64Encode();
        var solutionUpdate = await _solutionRepository.UpdateSolution(solutionMapper);
        solutionUpdate.ApplicationUser = user;
        var result = _mapper.Map<SolutionDto>(solutionUpdate);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticSolutions, result, solutionUpdate.Id);
        return new ApiSuccessResult<SolutionDto>(result);
    }

    public async Task<ApiResult<bool>> UpdateStatusSolution(int id)
    {
        var solutionEntity = await _solutionRepository.GetSolutionById(id);
        if (solutionEntity is null)
        {
            return new ApiResult<bool>(false, "Solution not found !!!");
        }

        solutionEntity.IsActive = !solutionEntity.IsActive;
        await _solutionRepository.UpdateSolution(solutionEntity);
        var result = _mapper.Map<SolutionDto>(solutionEntity);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticSolutions, result, id);
        return new ApiSuccessResult<bool>(true);
    }

    public async Task<ApiResult<bool>> DeleteSolution(int id)
    {
        var solutionEntity = await _solutionRepository.GetSolutionById(id);
        if (solutionEntity is null)
        {
            return new ApiResult<bool>(false, "Solution not found !!!");
        }

        var solutionDelete = await _solutionRepository.DeleteSolution(id);
        if (!solutionDelete)
        {
            return new ApiResult<bool>(false, "Failed to delete solution !!!");
        }

        _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticSolutions, id);
        return new ApiSuccessResult<bool>(true);
    }
}