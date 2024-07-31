using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.SolutionRepositories;

public class SolutionRepository : RepositoryBase<Solution, int>, ISolutionRepository
{
    public SolutionRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Solution>> GetAllSolution()
    {
        return await FindAll().ToListAsync();
    }

    public async Task<IEnumerable<Solution>> GetAllSolutionByProblemId(int problemId)
    {
        return await FindAll()
            .Include(p => p.Problem)
            .Include(u => u.ApplicationUser)
            .Where(p => p.ProblemId == problemId)
            .ToListAsync();
    }

    public Task<IQueryable<Solution>> GetAllSolutionPagination()
    {
        var result = FindAll().AsQueryable();
        return Task.FromResult(result);
    }

    public async Task<Solution?> GetSolutionById(int id)
    {
        return await FindByCondition(x => x.Id == id)
            .Include(p => p.Problem)
            .Include(u => u.ApplicationUser)
            .FirstOrDefaultAsync();
    }

    public async Task<Solution> CreateSolution(Solution solution)
    {
        await CreateAsync(solution);
        return solution;
    }

    public async Task<Solution> UpdateSolution(Solution solution)
    {
        await UpdateAsync(solution);
        return solution;
    }

    public async Task<bool> DeleteSolution(int id)
    {
        var solution = await GetByIdAsync(id);
        if (solution == null)
        {
            return false;
        }

        await DeleteAsync(solution);
        return true;
    }
}