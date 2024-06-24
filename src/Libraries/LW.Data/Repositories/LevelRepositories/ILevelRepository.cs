using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.LevelRepositories;

public interface ILevelRepository : IRepositoryBase<Level,int>
{
   Task CreateLevel(Level level);
   Task UpdateLevel(Level level);
   Task<bool> DeleteLevel(int id);
   Task<Level> GetLevelById(int id);
   Task<IEnumerable<Level>> GetAllLevel();
   Task<IQueryable<Level>> GetAllLevelPagination();

}