using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.ExecuteCodeRepositories;

public class ExecuteCodeRepository : RepositoryBase<ExecuteCode, int>, IExecuteCodeRepository
{
    public ExecuteCodeRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<ExecuteCode>> GetAllExecuteCode()
    {
        return await FindAll().ToListAsync();
    }

    public async Task<IEnumerable<ExecuteCode>> GetAllExecuteCodeByProblemId(int problemId)
    {
        return await FindAll().Where(e => e.ProblemId == problemId).ToListAsync();
    }

    public Task<IQueryable<ExecuteCode>> GetAllExecuteCodePagination()
    {
        var result = FindAll().AsQueryable();
        return Task.FromResult(result);
    }

    public async Task<ExecuteCode?> GetExecuteCodeById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<ExecuteCode> CreateExecuteCode(ExecuteCode executeCode)
    {
        await CreateAsync(executeCode);
        return executeCode;
    }

    public async Task<ExecuteCode> UpdateExecuteCode(ExecuteCode executeCode)
    {
        await UpdateAsync(executeCode);
        return executeCode;
    }

    public async Task<bool> DeleteExecuteCode(int id)
    {
        var executeCode = await GetByIdAsync(id);
        if (executeCode == null)
        {
            return false;
        }

        await DeleteAsync(executeCode);
        return true;
    }

    public async Task<bool> DeleteRangeExecuteCode(IEnumerable<ExecuteCode> executeCode)
    {
        executeCode = executeCode.Where(l => l != null);
        if (!executeCode.Any())
        {
            return false;
        }

        await DeleteListAsync(executeCode);
        return true;
    }
}