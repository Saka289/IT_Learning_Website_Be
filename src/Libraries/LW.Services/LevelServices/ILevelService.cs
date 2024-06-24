using System.Collections;
using LW.Shared.DTOs.Level;
using LW.Shared.SeedWork;

namespace LW.Services.LevelServices;

public interface ILevelService
{
    public Task<ApiResult<bool>> Create(LevelDtoForCreate model);
    public Task<ApiResult<bool>> Update(LevelDtoForUpdate model);
    public Task<ApiResult<bool>> UpdateStatus(int id);
    public Task<ApiResult<bool>> Delete(int id);
    public Task<ApiResult<IEnumerable<LevelDto>>> GetAll();
    public Task<ApiResult<LevelDto>> GetById(int id);
    public Task<ApiResult<IEnumerable<LevelDto>>> SearchLevel(SearchLevelDto searchLevelDto);
    public Task<ApiResult<PagedList<LevelDto>>> GetAllLevelPagination(
        PagingRequestParameters pagingRequestParameters);
}