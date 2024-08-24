using Elasticsearch.Net;
using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Level = LW.Data.Entities.Level;

namespace LW.Data.Repositories.LevelRepositories;

public class LevelRepository: RepositoryBase<Level, int>, ILevelRepository
{
    public LevelRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Level>> GetAllLevel()
    {
        return await FindAll().ToListAsync();
    }
}