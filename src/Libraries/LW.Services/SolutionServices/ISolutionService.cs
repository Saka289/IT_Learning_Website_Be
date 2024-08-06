using LW.Shared.SeedWork;
using LW.Shared.Solution;

namespace LW.Services.SolutionServices;

public interface ISolutionService
{
    Task<ApiResult<IEnumerable<SolutionDto>>> GetAllSolutionByProblemId(int problemId);
    Task<ApiResult<IEnumerable<SolutionDto>>> SearchSolutionByProblemId(int problemId, SearchRequestValue searchRequestValue);
    Task<ApiResult<SolutionDto>> GetSolutionById(int id);
    Task<ApiResult<SolutionDto>> CreateSolution(SolutionCreateDto solutionCreateDto);
    Task<ApiResult<SolutionDto>> UpdateSolution(SolutionUpdateDto solutionUpdateDto);
    Task<ApiResult<bool>> UpdateStatusSolution(int id);
    Task<ApiResult<bool>> DeleteSolution(int id);
}