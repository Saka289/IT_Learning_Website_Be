using System.Collections;
using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.ProblemRepositories;
using LW.Data.Repositories.TestCaseRepositories;
using LW.Shared.DTOs.TestCase;
using LW.Shared.SeedWork;
using Serilog;

namespace LW.Services.TestCaseServices;

public class TestCaseService : ITestCaseService
{
    private readonly ITestCaseRepository _testCaseRepository;
    private readonly IProblemRepository _problemRepository;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public TestCaseService(ITestCaseRepository testCaseRepository, IMapper mapper, IProblemRepository problemRepository, ILogger logger)
    {
        _testCaseRepository = testCaseRepository;
        _mapper = mapper;
        _problemRepository = problemRepository;
        _logger = logger;
    }

    public async Task<ApiResult<IEnumerable<TestCaseDto>>> GetAllTestCase()
    {
        var testCaseList = await _testCaseRepository.GetAllTestCase();
        if (!testCaseList.Any())
        {
            return new ApiResult<IEnumerable<TestCaseDto>>(false, "Test case not found !!!");
        }

        var result = _mapper.Map<IEnumerable<TestCaseDto>>(testCaseList);
        return new ApiSuccessResult<IEnumerable<TestCaseDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<TestCaseDto>>> GetAllTestCaseByProblemId(int problemId)
    {
        var testCaseList = await _testCaseRepository.GetAllTestCaseByProblemId(problemId);
        if (!testCaseList.Any())
        {
            return new ApiResult<IEnumerable<TestCaseDto>>(false, "Test case not found !!!");
        }

        var result = _mapper.Map<IEnumerable<TestCaseDto>>(testCaseList);
        return new ApiSuccessResult<IEnumerable<TestCaseDto>>(result);
    }

    public async Task<ApiResult<TestCaseDto>> GetTestCaseById(int id)
    {
        var testCase = await _testCaseRepository.GetTestCaseById(id);
        if (testCase is null)
        {
            return new ApiResult<TestCaseDto>(false, "Test case not found !!!");
        }

        var result = _mapper.Map<TestCaseDto>(testCase);
        return new ApiSuccessResult<TestCaseDto>(result);
    }

    public async Task<ApiResult<TestCaseDto>> CreateTestCase(TestCaseCreateDto testCaseCreateDto)
    {
        var problem = await _problemRepository.GetProblemById(testCaseCreateDto.ProblemId);
        if (problem is null)
        {
            return new ApiResult<TestCaseDto>(false, "Problem not found !!!");
        }

        var testCaseMapper = _mapper.Map<TestCase>(testCaseCreateDto);
        var testCaseUpdate = await _testCaseRepository.CreateTestCase(testCaseMapper);
        var result = _mapper.Map<TestCaseDto>(testCaseUpdate);
        return new ApiSuccessResult<TestCaseDto>(result);
    }

    public async Task<ApiResult<TestCaseDto>> UpdateTestCase(TestCaseUpdateDto testCaseUpdateDto)
    {
        var problem = await _problemRepository.GetProblemById(testCaseUpdateDto.ProblemId);
        if (problem is null)
        {
            return new ApiResult<TestCaseDto>(false, "Problem not found !!!");
        }

        var testCase = await _testCaseRepository.GetTestCaseById(testCaseUpdateDto.Id);
        if (testCase is null)
        {
            return new ApiResult<TestCaseDto>(false, "Test case not found !!!");
        }

        var testCaseMapper = _mapper.Map(testCaseUpdateDto, testCase);
        var testCaseUpdate = await _testCaseRepository.UpdateTestCase(testCaseMapper);
        var result = _mapper.Map<TestCaseDto>(testCaseUpdate);
        return new ApiSuccessResult<TestCaseDto>(result);
    }

    public async Task<ApiResult<bool>> DeleteTestCase(int id)
    {
        var testCase = await _testCaseRepository.GetTestCaseById(id);
        if (testCase is null)
        {
            return new ApiResult<bool>(false, "Test case not found !!!");
        }

        var testCaseDelete = await _testCaseRepository.DeleteTestCase(id);
        if (!testCaseDelete)
        {
            return new ApiResult<bool>(false, "Failed to delete test case !!!");
        }

        return new ApiSuccessResult<bool>(testCaseDelete);
    }

    public async Task<ApiResult<bool>> CreateRangeTestCase(IEnumerable<TestCaseCreateDto> testCaseCreateDto)
    {
        var problem =
            await _problemRepository.GetProblemById(testCaseCreateDto.Select(t => t.ProblemId).FirstOrDefault());
        if (problem is null)
        {
            return new ApiResult<bool>(false, "Problem not found !!!");
        }

        var testCasesMapper = _mapper.Map<IEnumerable<TestCase>>(testCaseCreateDto);
        var testCaseCreate = await _testCaseRepository.CreateRangeTestCase(testCasesMapper);
        if (!testCaseCreate)
        {
            return new ApiResult<bool>(false, "Failed create test case !!!");
        }

        return new ApiSuccessResult<bool>(true);
    }

    public async Task<ApiResult<bool>> UpdateRangeTestCase(IEnumerable<TestCaseUpdateDto> testCaseUpdateDto)
    {
        var problemId = testCaseUpdateDto.Select(t => t.ProblemId).FirstOrDefault();
        var problem = await _problemRepository.GetProblemById(problemId);
        if (problem is null)
        {
            return new ApiResult<bool>(false, "Problem not found !!!");
        }

        var testCaseList = await _testCaseRepository.GetAllTestCaseByProblemId(problemId);
        if (!testCaseList.Any())
        {
            return new ApiResult<bool>(false, "Test case not found !!!");
        }
        
        var testCasesMapper = new List<TestCase>();
        foreach (var item in testCaseList)
        {
            testCasesMapper.Add(_mapper.Map(testCaseUpdateDto.FirstOrDefault(x => x.Id == item.Id), item));
        }
        
        var testCasesUpdate = await _testCaseRepository.UpdateRangeTestCase(testCasesMapper);
        if (!testCasesUpdate)
        {
            return new ApiResult<bool>(false, "Failed update test case !!!");
        }

        return new ApiSuccessResult<bool>(true);
    }

    public async Task<ApiResult<bool>> DeleteRangeTestCase(IEnumerable<int> ids)
    {
        if (!ids.Any())
        {
            return new ApiResult<bool>(false, "Test case not found !!!");
        }

        var listTestCases = new List<TestCase>();
        foreach (var item in ids)
        {
            var testCase = await _testCaseRepository.GetTestCaseById(item);
            if (testCase is not null)
            {
                listTestCases.Add(testCase);
            }
            _logger.Information($"Test case not found with id: {item}");
        }

        var testCasesDelete = await _testCaseRepository.DeleteRangeTestCase(listTestCases);
        if (!testCasesDelete)
        {
            return new ApiResult<bool>(false, "Failed to delete test case !!!");
        }

        return new ApiSuccessResult<bool>(true);
    }
}