using AutoMapper;
using LW.Data.Repositories.SubmissionRepositories;
using LW.Shared.DTOs.Submission;
using LW.Shared.SeedWork;

namespace LW.Services.SubmissionServices;
 
public class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IMapper _mapper;
    public Task<ApiResult<IEnumerable<SubmissionDto>>> GetAllSubmission(SubmissionRequestDto submissionRequestDto)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResult<SubmissionDto>> GetSubmission(SubmissionRequestDto submissionRequestDto)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResult<SubmissionDto>> SubmitProblem(SubmitProblemDto submitProblemDto)
    {
        throw new NotImplementedException();
    }
}