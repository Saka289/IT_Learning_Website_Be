using LW.Data.Entities;
using LW.Shared.DTOs.Compile;
using LW.Shared.DTOs.Member;
using LW.Shared.DTOs.Submission;
using LW.Shared.Enums;
using Microsoft.AspNetCore.Identity;

namespace LW.Services.Common.ModelMapping;

public static class ModelMappingExtensions
{
    public static MemberDto ToMemberDto(this ApplicationUser applicationUser, UserManager<ApplicationUser> userManager)
    {
        return new MemberDto
        {
            Id = applicationUser.Id,
            Email = applicationUser.Email,
            UserName = applicationUser.UserName,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            PhoneNumber = applicationUser.PhoneNumber,
            Dob = Convert.ToString(applicationUser.Dob),
            Image = applicationUser.Image,
            EmailConfirmed = applicationUser.EmailConfirmed,
            LockOutEnd = applicationUser.LockoutEnd,
            Roles = userManager.GetRolesAsync(applicationUser).Result.ToList()
        };
    }
    
    public static Submission ToSubmission(this CompileDto compileDto, SubmitProblemDto submitProblemDto)
    {
        return new Submission
        {
            SourceCode = submitProblemDto.SourceCode,
            Status = (EStatusSubmission)compileDto.status_id,
            ExecutionTime = compileDto.time,
            MemoryUsage = compileDto.memory,
            LanguageId = submitProblemDto.LanguageId,
            ProblemId = submitProblemDto.ProblemId,
            UserId = submitProblemDto.UserId,
        };
    }

    public static SubmissionDto ToSubmissionDto(this CompileDto compileDto, Submission submission, int testCaseId)
    {
        return new SubmissionDto
        {
            Id = submission.Id,
            TestCaseId = testCaseId,
            Input = compileDto.stdin,
            Output = compileDto.stdout,
            StatusId = compileDto.status_id,
            Status = ((EStatusSubmission)compileDto.status_id).ToString(),
            CompileOutput = compileDto.compile_output,
            ExpectedOutput = compileDto.expected_output,
            StandardError = compileDto.stderr,
            Message = compileDto.message,
            ExecutionTime = compileDto.time,
            LanguageId = submission.LanguageId,
            MemoryUsage = compileDto.memory,
            ProblemId = submission.ProblemId,
            SourceCode = compileDto.source_code,
            UserId = submission.UserId
        };
    }
}