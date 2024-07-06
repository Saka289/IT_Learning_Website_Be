using LW.Shared.DTOs.Quiz;
using LW.Shared.SeedWork;

namespace LW.Services.QuizServices;

public interface IQuizService
{
    Task<ApiResult<IEnumerable<QuizDto>>> GetAllQuiz();
    Task<ApiResult<IEnumerable<QuizDto>>> GetAllQuizByTopicId(int topicId);
    Task<ApiResult<IEnumerable<QuizDto>>> GetAllQuizByLessonId(int lessonId);
    Task<ApiResult<QuizDto>> GetQuizById(int id);
    Task<ApiResult<bool>> UpdateQuizStatus(int id);
    Task<ApiResult<QuizDto>> CreateQuiz(QuizCreateDto quizCreateDto);
    Task<ApiResult<QuizDto>> UpdateQuiz(QuizUpdateDto quizUpdateDto);
    Task<ApiResult<bool>> DeleteQuiz(int id);
}