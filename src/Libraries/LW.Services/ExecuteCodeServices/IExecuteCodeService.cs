using System.Collections;
using LW.Data.Entities;
using LW.Shared.DTOs.ExecuteCode;
using LW.Shared.SeedWork;

namespace LW.Services.ExecuteCodeServices;

public interface IExecuteCodeService
{
    Task<ApiResult<IEnumerable<ExecuteCodeDto>>> GetAllExecuteCode();
    Task<ApiResult<IEnumerable<ExecuteCodeDto>>> GetAllExecuteCodeByProblemId(int problemId, int? languageId);
    Task<ApiResult<ExecuteCodeDto>> GetExecuteCodeById(int id);
    Task<ApiResult<ExecuteCodeDto>> CreateExecuteCode(ExecuteCodeCreateDto executeCodeCreateDto);
    Task<ApiResult<ExecuteCodeDto>> UpdateExecuteCode(ExecuteCodeUpdateDto executeCodeUpdateDto);
    Task<ApiResult<bool>> DeleteExecuteCode(int id);
    Task<ApiResult<bool>> CreateRangeExecuteCode(IEnumerable<ExecuteCodeCreateDto> executeCodeCreateDto);
    Task<ApiResult<bool>> UpdateRangeExecuteCode(IEnumerable<ExecuteCodeUpdateDto> executeCodeUpdateDto);
    Task<ApiResult<bool>> DeleteRangeExecuteCode(IEnumerable<int> ids);
}