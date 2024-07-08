using LW.Data.Entities;
using LW.Shared.DTOs.QuizQuestion;
using LW.Shared.SeedWork;

namespace LW.Services.QuizQuestionServices;

public interface IQuizQuestionService
{
    Task<ApiResult<IEnumerable<QuizQuestionDto>>> GetAllQuizQuestion();
    Task<ApiResult<PagedList<QuizQuestionDto>>> GetAllQuizQuestionPagination(PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<PagedList<QuizQuestionDto>>> GetAllQuizQuestionByQuizIdPagination(int quizId, PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<QuizQuestionDto>> GetQuizQuestionById(int id);
    Task<ApiResult<QuizQuestionDto>> CreateQuizQuestion(QuizQuestionCreateDto quizQuestionCreateDto);
    Task<ApiResult<QuizQuestionDto>> UpdateQuizQuestion(QuizQuestionUpdateDto quizQuestionUpdateDto);
    Task<ApiResult<bool>> CreateRangeQuizQuestion(IEnumerable<QuizQuestionCreateDto> quizQuestionsCreateDto);
    Task<ApiResult<bool>> UpdateRangeQuizQuestion(IEnumerable<QuizQuestionUpdateDto> quizQuestionsUpdateDto);
    Task<ApiResult<bool>> UpdateStatusQuizQuestion(int id);
    Task<ApiResult<bool>> DeleteQuizQuestion(int id);
    
}