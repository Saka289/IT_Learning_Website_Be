using LW.Shared.DTOs;
using LW.Shared.SeedWork;

namespace LW.Services.UserGradeServices;

public interface IUserGradeService
{
    public Task<ApiResult<bool>> CreatUserGrade(UserGradeCreateDto model);
    public Task<ApiResult<bool>> CreatRangeUserGrade(IEnumerable<UserGradeCreateDto> models);
    public Task<ApiResult<bool>> UpdateUserGrade(UserGradeUpdateDto model);
    public Task<ApiResult<bool>> DeleteUserGrade(int id);
  //  public Task<ApiResult<bool>> DeleteRangeUserGrade(IEnumerable<int> ids);
    public Task<ApiResult<IEnumerable<UserGradeDto>>> GetAllUserGrade();
    public Task<ApiResult<UserGradeDto>> GeUserGradeById(int id);
}