using LW.Shared.DTOs.ExamAnswer;
using LW.Shared.DTOs.ExamCode;
using LW.Shared.SeedWork;

namespace LW.Services.ExamCodeServices;

public interface IExamCodeService
{
    Task<ApiResult<IEnumerable<ExamCodeDto>>> GetAllExamCode();
    Task<ApiResult<ExamCodeDto>> GetExamCodeById(int id);
    Task<ApiResult<IEnumerable<ExamCodeDto>>> GetExamCodeByExamId(int examId);
    Task<ApiResult<ExamCodeDto>> CreateExamCode(ExamCodeCreateDto examCodeCreateDto);
    Task<ApiResult<ExamCodeDto>> UpdateExamCode(ExamCodeUpdateDto examCodeUpdateDto);
    Task<ApiResult<bool>> DeleteExamCode(int id);
}