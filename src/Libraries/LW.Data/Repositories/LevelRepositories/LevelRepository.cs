using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.LevelRepositories;

public class LevelRepository : RepositoryBase<Level, int>, ILevelRepository
{
    public LevelRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public Task CreateLevel(Level level)
    {
        return CreateAsync(level);
    }

    public Task UpdateLevel(Level level)
    {
        return UpdateAsync(level);
    }

    public async Task<bool> DeleteLevel(int id)
    {
        var level = await GetLevelById(id);
        if (level != null)
        {
            await DeleteAsync(level);
            return true;
        }

        return false;
    }

    public async Task<Level> GetLevelById(int id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<IEnumerable<Level>> GetAllLevel()
    {
        return await FindAll().ToListAsync();
    }

    public Task<IQueryable<Level>> GetAllLevelPagination()
    {
        var result = FindAll();
        return Task.FromResult(result);    }
}