using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.SubmissionRepositories;

public interface ISubmissionRepository : IRepositoryBase<Submission, int>
{
    Task<IEnumerable<Submission>> GetAllSubmission();
    Task<IEnumerable<Submission>> GetAllSubmissionByProblemIdUserId(int problemId, string userId);
    Task<bool> GetAllSubmissionByStatus(string userId, int problemId);
    Task<Submission?> GetSubmissionByProblemIdUserIdLanguageId(int problemId, string userId, int languageId);
    Task<Submission?> GetSubmissionByProblemIdUserId(int problemId, string userId);
    Task<IQueryable<Submission>> GetAllSubmissionPagination();
    Task<Submission?> GetSubmissionById(int id);
    Task<Submission> CreateSubmission(Submission submission);
    Task<Submission> UpdateSubmission(Submission submission);
    Task<bool> DeleteSubmission(int id);
}