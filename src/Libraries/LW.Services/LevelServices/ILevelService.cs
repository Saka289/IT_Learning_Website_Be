using LW.Shared.DTOs.Level;
using LW.Shared.SeedWork;

namespace LW.Services.LevelServices;

public interface ILevelService
{
    Task<ApiResult<IEnumerable<LevelDto>>> GetAllLevel();
}