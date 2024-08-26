using LW.Shared.DTOs.Exam;
using LW.Shared.DTOs.File;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Tag;
using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Services.ExamServices;

public interface IExamService
{
    Task<ApiResult<IEnumerable<ExamDto>>> GetAllExam(bool? status);
    Task<ApiResult<PagedList<ExamDto>>> GetAllExamPagination(SearchExamDto searchExamDto);
    Task<ApiResult<IEnumerable<TagDto>>> GetExamIdByTag(int id);
    Task<ApiResult<ExamDto>> GetExamById(int id);
    Task<ApiResult<IEnumerable<ExamDto>>> GetExamByType(EExamType type, bool? status);
    Task<ApiResult<ExamDto>> CreateExam(ExamCreateDto examCreateDto);
    Task<ApiResult<ExamDto>> UpdateExam(ExamUpdateDto examUpdateDto);
    Task<ApiResult<bool>> UpdateExamStatus(int id);
    Task<ApiResult<bool>> DeleteExam(int id);

}