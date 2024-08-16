using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.ProblemRepositories;

public class ProblemRepository : RepositoryBase<Problem, int>, IProblemRepository
{
    public ProblemRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Problem>> GetAllProblem()
    {
        return await FindAll()
            .Include(s => s.Submissions)
            .ToListAsync();
    }

    public async Task<IEnumerable<Problem>> GetAllProblemCustom()
    {
        return await FindAll().Where(p => p.TopicId == null && p.LessonId == null).ToListAsync();
    }

    public async Task<IEnumerable<Problem>> GetAllProblemByTopic(int topicId)
    {
        return await FindAll().Where(x => x.TopicId == topicId).ToListAsync();
    }

    public async Task<IEnumerable<Problem>> GetAllProblemByLesson(int lessonId)
    {
        return await FindAll().Where(x => x.LessonId == lessonId).ToListAsync();
    }

    public async Task<IEnumerable<Problem>> SearchProblemByTag(string tag, bool order)
    {
        var result = order
            ? await FindAll()
                .Where(p => p.KeyWord.Contains(tag)).OrderByDescending(p => p.CreatedDate).ToListAsync()
            : await FindAll()
                .Where(p => p.KeyWord.Contains(tag)).ToListAsync();
        return result;
    }

    public async Task<Problem?> GetProblemById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Problem> CreateProblem(Problem problem)
    {
        await CreateAsync(problem);
        return problem;
    }

    public async Task<Problem> UpdateProblem(Problem problem)
    {
        await UpdateAsync(problem);
        return problem;
    }

    public async Task<bool> DeleteProblem(int id)
    {
        var problem = await GetByIdAsync(id);
        if (problem == null)
        {
            return false;
        }

        await DeleteAsync(problem);
        return true;
    }

    public async Task<bool> DeleteRangeProblem(IEnumerable<Problem> problems)
    {
        problems = problems.Where(p => p != null);
        if (!problems.Any())
        {
            return false;
        }

        await DeleteListAsync(problems);
        return true;
    }
}