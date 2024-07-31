using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.SubmissionRepositories;

public interface ISubmissionRepository : IRepositoryBase<Submission, int>
{
    Task<IEnumerable<Submission>> GetAllSubmission();
    Task<IQueryable<Submission>> GetAllSubmissionPagination();
    Task<Submission?> GetSubmissionById(int id);
    Task<Submission> CreateSubmission(Submission submission);
    Task<Submission> UpdateSubmission(Submission submission);
    Task<bool> DeleteSubmission(int id);
}