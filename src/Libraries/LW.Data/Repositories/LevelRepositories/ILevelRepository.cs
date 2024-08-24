using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.LevelRepositories;

public interface ILevelRepository: IRepositoryBase<Level, int>
{
    Task<IEnumerable<Level>> GetAllLevel();
}