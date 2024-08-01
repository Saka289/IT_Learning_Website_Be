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
}