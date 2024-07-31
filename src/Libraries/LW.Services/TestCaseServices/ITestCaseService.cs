using LW.Shared.DTOs.TestCase;
using LW.Shared.SeedWork;

namespace LW.Services.TestCaseServices;

public interface ITestCaseService
{
    Task<ApiResult<IEnumerable<TestCaseDto>>> GetAllTestCase();
    Task<ApiResult<IEnumerable<TestCaseDto>>> GetAllTestCaseByProblemId(int problemId);
    Task<ApiResult<TestCaseDto>> GetTestCaseById(int id);
    Task<ApiResult<TestCaseDto>> CreateTestCase(TestCaseCreateDto testCaseCreateDto);
    Task<ApiResult<TestCaseDto>> UpdateTestCase(TestCaseUpdateDto testCaseUpdateDto);
    Task<ApiResult<bool>> DeleteTestCase(int id);
    Task<ApiResult<bool>> CreateRangeTestCase(IEnumerable<TestCaseCreateDto> testCaseCreateDto);
    Task<ApiResult<bool>> UpdateRangeTestCase(IEnumerable<TestCaseUpdateDto> testCaseUpdateDto);
    Task<ApiResult<bool>> DeleteRangeTestCase(IEnumerable<int> ids);
}