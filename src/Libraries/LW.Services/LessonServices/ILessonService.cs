using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Tag;
using LW.Shared.SeedWork;

namespace LW.Services.LessonServices;

public interface ILessonService
{
    Task<ApiResult<IEnumerable<LessonDto>>> GetAllLesson(bool? status);
    Task<ApiResult<IEnumerable<LessonDto>>> GetAllLessonByTopic(int id, bool? status);
    Task<ApiResult<IEnumerable<TagDto>>> GetLessonIdByTag(int id);
    Task<ApiResult<PagedList<LessonDto>>> GetAllLessonPagination(SearchLessonDto searchLessonDto);
    Task<ApiResult<LessonDto>> GetLessonById(int id);
    Task<ApiResult<LessonDto>> CreateLesson(LessonCreateDto lessonCreateDto);
    Task<ApiResult<LessonDto>> UpdateLesson(LessonUpdateDto lessonUpdateDto);
    Task<ApiResult<bool>> UpdateLessonStatus(int id);
    Task<ApiResult<bool>> DeleteLesson(int id);
    Task<ApiResult<bool>> DeleteRangeLesson(IEnumerable<int> ids);
}