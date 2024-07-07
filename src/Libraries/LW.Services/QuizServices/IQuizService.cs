using LW.Shared.DTOs.Quiz;
using LW.Shared.SeedWork;

namespace LW.Services.QuizServices;

public interface IQuizService
{
    Task<ApiResult<IEnumerable<QuizDto>>> GetAllQuiz();
    Task<ApiResult<PagedList<QuizDto>>> GetAllQuizPagination(PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<PagedList<QuizDto>>> GetAllQuizByTopicIdPagination(int topicId, PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<PagedList<QuizDto>>> GetAllQuizByLessonIdPagination(int lessonId, PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<QuizDto>> GetQuizById(int id);
    Task<ApiResult<bool>> UpdateQuizStatus(int id);
    Task<ApiResult<QuizDto>> CreateQuiz(QuizCreateDto quizCreateDto);
    Task<ApiResult<QuizDto>> UpdateQuiz(QuizUpdateDto quizUpdateDto);
    Task<ApiResult<bool>> DeleteQuiz(int id);
}