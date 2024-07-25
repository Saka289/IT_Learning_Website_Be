using LW.Shared.DTOs.Quiz;
using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Services.QuizServices;

public interface IQuizService
{
    Task<ApiResult<IEnumerable<QuizDto>>> GetAllQuiz();
    Task<ApiResult<PagedList<QuizDto>>> GetAllQuizPagination(ETypeQuiz typeQuiz,PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<PagedList<QuizDto>>> GetAllQuizByTopicIdPagination(int topicId, ETypeQuiz typeQuiz, PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<PagedList<QuizDto>>> GetAllQuizByLessonIdPagination(int lessonId,ETypeQuiz typeQuiz, PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<PagedList<QuizDto>>> SearchQuizPagination(SearchQuizDto searchQuizDto);
    Task<ApiResult<QuizDto>> GetQuizById(int id);
    Task<ApiResult<bool>> UpdateQuizStatus(int id);
    Task<ApiResult<QuizDto>> CreateQuiz(QuizCreateDto quizCreateDto);
    Task<ApiResult<QuizDto>> UpdateQuiz(QuizUpdateDto quizUpdateDto);
    Task<ApiResult<bool>> DeleteQuiz(int id);
}