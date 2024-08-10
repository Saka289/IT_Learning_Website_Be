using LW.Shared.DTOs.Solution;
using LW.Shared.SeedWork;

namespace LW.Services.SolutionServices;

public interface ISolutionService
{
    Task<ApiResult<IEnumerable<SolutionDto>>> GetAllSolutionByProblemId(SearchSolutionDto searchSolutionDto);
    Task<ApiResult<SolutionDto>> GetSolutionById(int id);
    Task<ApiResult<SolutionDto>> CreateSolution(SolutionCreateDto solutionCreateDto);
    Task<ApiResult<SolutionDto>> UpdateSolution(SolutionUpdateDto solutionUpdateDto);
    Task<ApiResult<bool>> UpdateStatusSolution(int id);
    Task<ApiResult<bool>> DeleteSolution(int id);
}