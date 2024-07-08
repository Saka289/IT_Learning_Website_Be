using LW.Shared.DTOs;
using LW.Shared.DTOs.Exam;
using LW.Shared.SeedWork;

namespace LW.Services.ExamImageServices;

public interface IExamImageService
{
    Task<ApiResult<IEnumerable<ExamImageDto>>> GetAllExamImage();
    Task<ApiResult<ExamImageDto>> GetExamImageById(int id);
    Task<ApiResult<IEnumerable<ExamImageDto>>> GetExamImageByExamId(int examId);
    
    Task<ApiResult<ExamImageDto>> CreateExamImage(ExamImageCreateDto examImageCreateDto);
    
    Task<ApiResult<ExamImageDto>> UpdateExamImage(ExamImageUpdateDto examImageUpdateDto);
    Task<ApiResult<bool>> DeleteExamImage(int id);
}