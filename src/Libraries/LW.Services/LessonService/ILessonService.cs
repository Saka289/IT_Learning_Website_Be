using LW.Shared.DTOs.Lesson;
using LW.Shared.SeedWork;

namespace LW.Services.LessonService;

public interface ILessonService
{
    Task<ApiResult<IEnumerable<LessonDto>>> GetAllLesson();
    Task<ApiResult<PagedList<LessonDto>>> GetAllLessonPagination(PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<LessonDto>> GetLessonById(int id);
    Task<ApiResult<IEnumerable<LessonDto>>> SearchByLesson(SearchLessonDto searchLessonDto);
    Task<ApiResult<LessonDto>> CreateLesson(LessonCreateDto lessonCreateDto);
    Task<ApiResult<LessonDto>> UpdateLesson(LessonUpdateDto lessonUpdateDto);
    Task<ApiResult<bool>> UpdateLessonStatus(int id);
    Task<ApiResult<bool>> DeleteLesson(int id);
}