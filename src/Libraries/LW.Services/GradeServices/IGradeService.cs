using LW.Data.Entities;
using LW.Shared.DTOs.Grade;
using LW.Shared.SeedWork;

namespace LW.Services.GradeServices;

public interface IGradeService
{
    Task<ApiResult<IEnumerable<GradeDto>>> GetAllGrade();
    Task<ApiResult<PagedList<GradeDto>>> GetAllGradePagination(SearchGradeDto searchGradeDto);
    Task<ApiResult<GradeDto>> GetGradeById(int id);
    Task<ApiResult<GradeDto>> CreateGrade(GradeCreateDto gradeCreateDto);
    Task<ApiResult<GradeDto>> UpdateGrade(GradeUpdateDto gradeUpdateDto);
    Task<ApiResult<bool>> UpdateGradeStatus(int id);
    Task<ApiResult<bool>> DeleteGrade(int id);
}