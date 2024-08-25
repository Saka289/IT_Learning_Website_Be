using LW.Shared.DTOs.Quiz;
using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Services.QuizServices;

public interface IQuizService
{
    Task<ApiResult<IEnumerable<QuizDto>>> GetAllQuiz(bool? status);
    Task<ApiResult<PagedList<QuizDto>>> GetAllQuizPagination(SearchQuizDto searchQuizDto);
    Task<ApiResult<IEnumerable<QuizDto>>> GetAllQuizNoPagination(SearchQuizDto searchQuizDto);
    Task<ApiResult<QuizDto>> GetQuizById(int id);
    Task<ApiResult<bool>> UpdateQuizStatus(int id);
    Task<ApiResult<QuizDto>> CreateQuiz(QuizCreateDto quizCreateDto);
    Task<ApiResult<QuizDto>> UpdateQuiz(QuizUpdateDto quizUpdateDto);
    Task<ApiResult<bool>> DeleteQuiz(int id);
}