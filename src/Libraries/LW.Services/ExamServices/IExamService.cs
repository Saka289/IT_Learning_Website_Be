using LW.Shared.DTOs.Exam;
using LW.Shared.DTOs.Lesson;
using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Services.ExamServices;

public interface IExamService
{
    Task<ApiResult<IEnumerable<ExamDto>>> GetAllExam();
    Task<ApiResult<PagedList<ExamDto>>> GetAllExamPagination(PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<ExamDto>> GetExamById(int id);
    Task<ApiResult<IEnumerable<ExamDto>>> GetExamByType(EExamType type);
    Task<ApiResult<PagedList<ExamDto>>> SearchByExamPagination(SearchExamDto searchExamDto);
    Task<ApiResult<ExamDto>> CreateExam(ExamCreateDto examCreateDto);
    Task<ApiResult<ExamDto>> UpdateExam(ExamUpdateDto examUpdateDto);
    Task<ApiResult<bool>> UpdateExamStatus(int id);
    Task<ApiResult<bool>> DeleteExam(int id);
}