﻿using LW.Shared.DTOs.Problem;
using LW.Shared.SeedWork;

namespace LW.Services.ProblemServices;

public interface IProblemService
{
    Task<ApiResult<IEnumerable<ProblemDto>>> GetAllProblem();

    Task<ApiResult<PagedList<ProblemDto>>> GetAllProblemPagination(SearchProblemDto searchProblemDto);

    Task<ApiResult<ProblemDto>> GetProblemById(int id);

    Task<ApiResult<ProblemDto>> CreateProblem(ProblemCreateDto problemCreateDto);

    Task<ApiResult<ProblemDto>> UpdateProblem(ProblemUpdateDto problemUpdateDto);

    Task<ApiResult<bool>> UpdateStatusProblem(int id);

    Task<ApiResult<bool>> DeleteProblem(int id);
}