﻿using LW.Data.Entities;
using LW.Shared.DTOs.Grade;
using LW.Shared.SeedWork;

namespace LW.Services.GradeServices;

public interface IGradeService
{
    Task<ApiResult<IEnumerable<GradeDto>>> GetAllGrade(bool isInclude = false);
    Task<ApiResult<PagedList<GradeDto>>> GetAllGradePagination(SearchGradeDto searchGradeDto);
    Task<ApiResult<GradeDto>> GetGradeById(int id, bool isInclude = false);
    Task<ApiResult<GradeDto>> CreateGrade(GradeCreateDto gradeCreateDto);
    Task<ApiResult<GradeDto>> UpdateGrade(GradeUpdateDto gradeUpdateDto);
    Task<ApiResult<bool>> UpdateGradeStatus(int id);
    Task<ApiResult<bool>> DeleteGrade(int id);
    Task<ApiResult<IEnumerable<GradeDto>>> GetListGradeByLevelId(int levelId);

}