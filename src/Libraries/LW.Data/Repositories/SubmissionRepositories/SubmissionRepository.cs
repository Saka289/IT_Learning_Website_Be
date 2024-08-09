using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using LW.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.SubmissionRepositories;

public class SubmissionRepository : RepositoryBase<Submission, int>, ISubmissionRepository
{
    public SubmissionRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Submission>> GetAllSubmission()
    {
        return await FindAll().ToListAsync();
    }

    public async Task<IEnumerable<Submission>> GetAllSubmissionByProblemIdUserId(int problemId, string userId)
    {
        return await FindAll()
            .Where(s => s.ProblemId == problemId && s.UserId.Equals(userId) && s.Submit)
            .ToListAsync();
    }

    public async Task<bool> GetAllSubmissionByStatus(string userId, int problemId)
    {
        return await FindAll().AnyAsync(s => s.ProblemId == problemId && s.UserId.Equals(userId) && s.Submit);
    }

    public async Task<Submission?> GetSubmissionByProblemIdUserId(int problemId, string userId, int languageId)
    {
        return await FindAll()
            .Where(s => s.ProblemId == problemId && s.UserId.Equals(userId) && s.LanguageId == languageId && s.Status == EStatusSubmission.Accepted && s.Submit)
            .OrderByDescending(s => s.CreatedDate)
            .FirstOrDefaultAsync();
    }

    public Task<IQueryable<Submission>> GetAllSubmissionPagination()
    {
        var result = FindAll().AsQueryable();
        return Task.FromResult(result);
    }

    public async Task<Submission?> GetSubmissionById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Submission> CreateSubmission(Submission submission)
    {
        await CreateAsync(submission);
        return submission;
    }

    public async Task<Submission> UpdateSubmission(Submission submission)
    {
        await UpdateAsync(submission);
        return submission;
    }

    public async Task<bool> DeleteSubmission(int id)
    {
        var submission = await GetByIdAsync(id);
        if (submission == null)
        {
            return false;
        }

        await DeleteAsync(submission);
        return true;
    }
}