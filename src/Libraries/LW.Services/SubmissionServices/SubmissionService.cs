using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.ExecuteCodeRepositories;
using LW.Data.Repositories.ProblemRepositories;
using LW.Data.Repositories.ProgramLanguageRepositories;
using LW.Data.Repositories.SubmissionRepositories;
using LW.Data.Repositories.TestCaseRepositories;
using LW.Infrastructure.Extensions;
using LW.Services.Common.CommonServices.CompileServices;
using LW.Services.Common.ModelMapping;
using LW.Shared.DTOs.Compile;
using LW.Shared.DTOs.Submission;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace LW.Services.SubmissionServices;

public class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly ITestCaseRepository _testCaseRepository;
    private readonly IExecuteCodeRepository _executeCodeRepository;
    private readonly IProblemRepository _problemRepository;
    private readonly IProgramLanguageRepository _programLanguageRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICompileService _compileService;
    private readonly IMapper _mapper;

    public SubmissionService(ISubmissionRepository submissionRepository, ICompileService compileService, IMapper mapper,
        ITestCaseRepository testCaseRepository, IExecuteCodeRepository executeCodeRepository,
        UserManager<ApplicationUser> userManager, IProblemRepository problemRepository,
        IProgramLanguageRepository programLanguageRepository)
    {
        _submissionRepository = submissionRepository;
        _compileService = compileService;
        _mapper = mapper;
        _testCaseRepository = testCaseRepository;
        _executeCodeRepository = executeCodeRepository;
        _userManager = userManager;
        _problemRepository = problemRepository;
        _programLanguageRepository = programLanguageRepository;
    }

    public async Task<ApiResult<IEnumerable<SubmissionDto>>> GetAllSubmission(SubmissionRequestDto submissionRequestDto)
    {
        var submissionList = await _submissionRepository.GetAllSubmissionByProblemIdUserId(submissionRequestDto.ProblemId, submissionRequestDto.UserId);
        if (!submissionList.Any())
        {
            return new ApiResult<IEnumerable<SubmissionDto>>(false, "Submission not found !!!");
        }

        var result = _mapper.Map<IEnumerable<SubmissionDto>>(submissionList);
        return new ApiSuccessResult<IEnumerable<SubmissionDto>>(result);
    }

    public async Task<ApiResult<SubmissionDto>> GetSubmission(SubmissionRequestDto submissionRequestDto)
    {
        var submission = await _submissionRepository.GetSubmissionByProblemIdUserId(submissionRequestDto.ProblemId, submissionRequestDto.UserId, submissionRequestDto.LanguageId);
        if (submission is null)
        {
            return new ApiResult<SubmissionDto>(false, "Submission not found !!!");
        }

        var result = _mapper.Map<SubmissionDto>(submission);
        return new ApiSuccessResult<SubmissionDto>(result);
    }

    public async Task<ApiResult<IEnumerable<SubmissionDto>>> SubmitProblem(SubmitProblemDto submitProblemDto)
    {
        var problem = await _problemRepository.GetProblemById(submitProblemDto.ProblemId);
        if (problem is null)
        {
            return new ApiResult<IEnumerable<SubmissionDto>>(false, "Problem not found");
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(submitProblemDto.UserId));
        if (user is null)
        {
            return new ApiResult<IEnumerable<SubmissionDto>>(false, "User not found");
        }

        var language = await _programLanguageRepository.GetProgramLanguageById(submitProblemDto.LanguageId);
        if (language is null)
        {
            return new ApiResult<IEnumerable<SubmissionDto>>(false, "Language not found !!!");
        }

        var executeCode = await _executeCodeRepository.GetExecuteCodeByProblemIdLanguageId(submitProblemDto.ProblemId, submitProblemDto.LanguageId);
        if (executeCode is null)
        {
            return new ApiResult<IEnumerable<SubmissionDto>>(false, "Execute not found !!!");
        }

        var testCases = await _testCaseRepository.GetAllTestCaseByProblemId(submitProblemDto.ProblemId);
        if (!testCases.Any())
        {
            return new ApiResult<IEnumerable<SubmissionDto>>(false, "TestCases not found !!!");
        }

        var sourceCode = (executeCode.Libraries?.Base64Decode() + "\n" + submitProblemDto.SourceCode.Base64Decode() + "\n" + executeCode.MainCode?.Base64Decode()).Base64Encode();
        if (!sourceCode.IsBase64String())
        {
            return new ApiResult<IEnumerable<SubmissionDto>>(false, "This is not base64 string !!!");
        }

        var result = await ProcessTestCases(submitProblemDto, testCases, language, sourceCode);

        return result;
    }

    private async Task<ApiResult<IEnumerable<SubmissionDto>>> ProcessTestCases(SubmitProblemDto submitProblemDto,
        IEnumerable<TestCase> testCases, ProgramLanguage programLanguage, string sourceCode)
    {
        var submission = new CompileDto();
        var submissionList = new List<SubmissionDto>();
        foreach (var item in submitProblemDto.Submit ? testCases : testCases.Where(t => !t.IsHidden))
        {
            submission = await _compileService.SubmitCompile(new CompileCreateDto()
            {
                language_id = programLanguage.BaseId,
                source_code = sourceCode,
                stdin = item.Input ?? null,
                expected_output = item.Output
            });
            if (submission.status_id != (int)EStatusSubmission.Accepted ||
                submission.status_id == (int)EStatusSubmission.WrongAnswer)
            {
                var submissionCreateFail =
                    await _submissionRepository.CreateSubmission(submission.ToSubmission(submitProblemDto));
                submissionList.Add(submission.ToSubmissionDto(submissionCreateFail, item.Id));
                return new ApiResult<IEnumerable<SubmissionDto>>(false, submissionList,
                    ((EStatusSubmission)submission.status_id).ToString());
            }

            var submissionCreateSuccess =
                await _submissionRepository.CreateSubmission(submission.ToSubmission(submitProblemDto));
            submissionList.Add(submission.ToSubmissionDto(submissionCreateSuccess, item.Id));
        }

        if (submitProblemDto.Submit)
        {
            var submissionEntity =
                await _submissionRepository.GetSubmissionById(submissionList.Select(x => x.Id).Last());
            if (submissionEntity is null)
            {
                return new ApiResult<IEnumerable<SubmissionDto>>(false, "Submit problem failed !!!");
            }

            submissionEntity.Submit = submitProblemDto.Submit;
            await _submissionRepository.UpdateSubmission(submissionEntity);
        }

        return new ApiResult<IEnumerable<SubmissionDto>>(true, submissionList,
            ((EStatusSubmission)submission.status_id).ToString());
    }
}