using LW.Data.Entities;
using LW.Shared.DTOs.Submission;
using LW.Shared.SeedWork;

namespace LW.Services.SubmissionServices;

public interface ISubmissionService
{
    Task<ApiResult<IEnumerable<SubmissionDto>>> GetAllSubmission(SubmissionRequestDto submissionRequestDto);
    Task<ApiResult<SubmissionDto>> GetSubmission(SubmissionRequestDto submissionRequestDto);
    Task<ApiResult<SubmissionDto>> SubmitProblem(SubmitProblemDto submitProblemDto);
}