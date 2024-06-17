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
    public Task<Level> GetLevelById(int id)
    {
        return GetByIdAsync(id);
    }

    public async Task<IEnumerable<Level>> GetAllLevel()
    {
        return await FindAll().ToListAsync();
    }

    public async Task<bool> UpdateStatusLevel(int id)
    {
        var level =  await GetLevelById(id);
        if (level != null)
        {
            // level.Active = false;
            await UpdateAsync(level);
            return true;
        }
        return false;
    }
}