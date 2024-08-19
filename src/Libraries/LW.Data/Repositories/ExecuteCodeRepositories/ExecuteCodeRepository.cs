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
        return await FindAll()
            .Include(l => l.ProgramLanguage)
            .ToListAsync();
    }

    public async Task<IEnumerable<ExecuteCode>> GetAllExecuteCodeByProblemId(int problemId)
    {
        return await FindAll()
            .Include(l => l.ProgramLanguage)
            .Where(e => e.ProblemId == problemId).ToListAsync();
    }

    public Task<IQueryable<ExecuteCode>> GetAllExecuteCodePagination()
    {
        var result = FindAll()
            .Include(l => l.ProgramLanguage)
            .AsQueryable();
        return Task.FromResult(result);
    }

    public async Task<ExecuteCode?> GetExecuteCodeById(int id)
    {
        return await FindByCondition(x => x.Id == id)
            .Include(l => l.ProgramLanguage)
            .FirstOrDefaultAsync();
    }

    public async Task<ExecuteCode?> GetExecuteCodeByProblemIdLanguageId(int problemId, int languageId)
    {
        return await FindAll()
            .Include(l => l.ProgramLanguage)
            .FirstOrDefaultAsync(e => e.ProblemId == problemId && e.LanguageId == languageId);
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