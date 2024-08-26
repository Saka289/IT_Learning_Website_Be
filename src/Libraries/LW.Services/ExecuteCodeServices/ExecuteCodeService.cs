using System.Collections;
using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.ExecuteCodeRepositories;
using LW.Data.Repositories.ProblemRepositories;
using LW.Data.Repositories.ProgramLanguageRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.DTOs.ExecuteCode;
using LW.Shared.SeedWork;
using Serilog;

namespace LW.Services.ExecuteCodeServices;

public class ExecuteCodeService : IExecuteCodeService
{
    private readonly IExecuteCodeRepository _executeCodeRepository;
    private readonly IProblemRepository _problemRepository;
    private readonly IProgramLanguageRepository _programLanguageRepository;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public ExecuteCodeService(IExecuteCodeRepository executeCodeRepository, IMapper mapper,
        IProblemRepository problemRepository, IProgramLanguageRepository programLanguageRepository, ILogger logger)
    {
        _executeCodeRepository = executeCodeRepository;
        _mapper = mapper;
        _problemRepository = problemRepository;
        _programLanguageRepository = programLanguageRepository;
        _logger = logger;
    }

    public async Task<ApiResult<IEnumerable<ExecuteCodeDto>>> GetAllExecuteCode()
    {
        var executeCodeList = await _executeCodeRepository.GetAllExecuteCode();
        if (!executeCodeList.Any())
        {
            return new ApiResult<IEnumerable<ExecuteCodeDto>>(false, "Execute code not found !!!");
        }

        var result = _mapper.Map<IEnumerable<ExecuteCodeDto>>(executeCodeList);
        return new ApiSuccessResult<IEnumerable<ExecuteCodeDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<ExecuteCodeDto>>> GetAllExecuteCodeByProblemId(int problemId,
        int? languageId)
    {
        var executeCodeList = await _executeCodeRepository.GetAllExecuteCodeByProblemId(problemId);
        if (!executeCodeList.Any())
        {
            return new ApiResult<IEnumerable<ExecuteCodeDto>>(false, "Execute code not found !!!");
        }

        if (languageId > 0)
        {
            executeCodeList = executeCodeList.Where(e => e.LanguageId == languageId);
        }

        var result = _mapper.Map<IEnumerable<ExecuteCodeDto>>(executeCodeList);
        return new ApiSuccessResult<IEnumerable<ExecuteCodeDto>>(result);
    }

    public async Task<ApiResult<ExecuteCodeDto>> GetExecuteCodeById(int id)
    {
        var executeCode = await _executeCodeRepository.GetExecuteCodeById(id);
        if (executeCode is null)
        {
            return new ApiResult<ExecuteCodeDto>(false, "Execute not found !!!");
        }

        var result = _mapper.Map<ExecuteCodeDto>(executeCode);
        return new ApiSuccessResult<ExecuteCodeDto>(result);
    }

    public async Task<ApiResult<ExecuteCodeDto>> CreateExecuteCode(ExecuteCodeCreateDto executeCodeCreateDto)
    {
        var problem = await _problemRepository.GetProblemById(executeCodeCreateDto.ProblemId);
        if (problem is null)
        {
            return new ApiResult<ExecuteCodeDto>(false, "Problem not found !!!");
        }

        var isDuplicate = await FindDuplicateExecuteCode(executeCodeCreateDto.ProblemId, executeCodeCreateDto.LanguageId, true);
        if (!isDuplicate)
        {
            return new ApiResult<ExecuteCodeDto>(false, "ExecuteCode is duplicate !!!");
        }

        var language = await _programLanguageRepository.GetProgramLanguageById(executeCodeCreateDto.LanguageId);
        if (language is null)
        {
            return new ApiResult<ExecuteCodeDto>(false, "Languages not found !!!");
        }

        var executeCodeMapper = _mapper.Map<ExecuteCode>(executeCodeCreateDto);
        var executeCodeCreate = await _executeCodeRepository.CreateExecuteCode(executeCodeMapper);
        var result = _mapper.Map<ExecuteCodeDto>(executeCodeCreate);
        return new ApiSuccessResult<ExecuteCodeDto>(result);
    }

    public async Task<ApiResult<ExecuteCodeDto>> UpdateExecuteCode(ExecuteCodeUpdateDto executeCodeUpdateDto)
    {
        var problem = await _problemRepository.GetProblemById(executeCodeUpdateDto.ProblemId);
        if (problem is null)
        {
            return new ApiResult<ExecuteCodeDto>(false, "Problem not found !!!");
        }

        var language = await _programLanguageRepository.GetProgramLanguageById(executeCodeUpdateDto.LanguageId);
        if (language is null)
        {
            return new ApiResult<ExecuteCodeDto>(false, "Languages not found !!!");
        }

        var executeCode = await _executeCodeRepository.GetExecuteCodeById(executeCodeUpdateDto.Id);
        if (executeCode is null)
        {
            return new ApiResult<ExecuteCodeDto>(false, "Execute not found !!!");
        }
        
        var isDuplicate = await FindDuplicateExecuteCode(executeCodeUpdateDto.ProblemId, executeCodeUpdateDto.LanguageId, false, executeCode.LanguageId);
        if (!isDuplicate)
        {
            return new ApiResult<ExecuteCodeDto>(false, "ExecuteCode is duplicate !!!");
        }
        
        var executeCodeMapper = _mapper.Map(executeCodeUpdateDto, executeCode);
        var executeCodeCreate = await _executeCodeRepository.UpdateExecuteCode(executeCodeMapper);
        var result = _mapper.Map<ExecuteCodeDto>(executeCodeCreate);
        return new ApiSuccessResult<ExecuteCodeDto>(result);
    }

    public async Task<ApiResult<bool>> DeleteExecuteCode(int id)
    {
        var executeCode = await _executeCodeRepository.GetExecuteCodeById(id);
        if (executeCode is null)
        {
            return new ApiResult<bool>(false, "Execute code not found !!!");
        }

        var executeCodeDelete = await _executeCodeRepository.DeleteExecuteCode(id);
        if (!executeCodeDelete)
        {
            return new ApiSuccessResult<bool>(false, "Failed to delete execute code !!!");
        }

        return new ApiSuccessResult<bool>(executeCodeDelete);
    }

    public async Task<ApiResult<bool>> CreateRangeExecuteCode(IEnumerable<ExecuteCodeCreateDto> executeCodeCreateDto)
    {
        foreach (var item in executeCodeCreateDto)
        {
            var problem = await _problemRepository.GetProblemById(item.ProblemId);
            if (problem is null)
            {
                return new ApiResult<bool>(false, "Problem not found !!!");
            }
            
            var isDuplicate = await FindDuplicateExecuteCode(item.ProblemId, item.LanguageId, true);
            if (!isDuplicate)
            {
                return new ApiResult<bool>(false, "ExecuteCode is duplicate !!!");
            }

            var language = await _programLanguageRepository.GetProgramLanguageById(item.LanguageId);
            if (language is null)
            {
                return new ApiResult<bool>(false, "Languages not found !!!");
            }

            var executeCodeMapper = _mapper.Map<ExecuteCode>(item);
            await _executeCodeRepository.CreateExecuteCode(executeCodeMapper);
        }

        return new ApiSuccessResult<bool>(true);
    }

    public async Task<ApiResult<bool>> UpdateRangeExecuteCode(IEnumerable<ExecuteCodeUpdateDto> executeCodeUpdateDto)
    {
        foreach (var item in executeCodeUpdateDto)
        {
            var problem = await _problemRepository.GetProblemById(item.ProblemId);
            if (problem is null)
            {
                return new ApiResult<bool>(false, "Problem not found !!!");
            }

            var language = await _programLanguageRepository.GetProgramLanguageById(item.LanguageId);
            if (language is null)
            {
                return new ApiResult<bool>(false, "Languages not found !!!");
            }

            var executeCode = await _executeCodeRepository.GetExecuteCodeById(item.Id);
            if (executeCode is null)
            {
                return new ApiResult<bool>(false, "Execute not found !!!");
            }
            
            var isDuplicate = await FindDuplicateExecuteCode(item.ProblemId, item.LanguageId, false, executeCode.LanguageId);
            if (!isDuplicate)
            {
                return new ApiResult<bool>(false, "ExecuteCode is duplicate !!!");
            }

            var executeCodeMapper = _mapper.Map(item, executeCode);
            await _executeCodeRepository.UpdateExecuteCode(executeCodeMapper);
        }

        return new ApiSuccessResult<bool>(true);
    }

    public async Task<ApiResult<bool>> DeleteRangeExecuteCode(IEnumerable<int> ids)
    {
        if (!ids.Any())
        {
            return new ApiResult<bool>(false, "Execute code not found !!!");
        }

        var listExecuteCode = new List<ExecuteCode>();
        foreach (var item in ids)
        {
            var executeCode = await _executeCodeRepository.GetExecuteCodeById(item);
            if (executeCode is not null)
            {
                listExecuteCode.Add(executeCode);
            }

            _logger.Information($"Execute code not found with id: {item}");
        }

        var executeCodeDelete = await _executeCodeRepository.DeleteRangeExecuteCode(listExecuteCode);
        if (!executeCodeDelete)
        {
            return new ApiResult<bool>(false, "Failed to delete execute code !!!");
        }

        return new ApiSuccessResult<bool>(true);
    }

    private async Task<bool> FindDuplicateExecuteCode(int problemId, int languageIdNew, bool createOrUpdate, int languageIdOld = 0)
    {
        var executeCodeList = await _executeCodeRepository.GetAllExecuteCodeByProblemId(problemId);
        if (!createOrUpdate)
        {
            executeCodeList = executeCodeList.Where(e => e.LanguageId != languageIdOld);
        }
        if (executeCodeList.Any(e => e.LanguageId == languageIdNew))
        {
            return false;
        }

        return true;
    }
}