using LW.Shared.DTOs.QuizQuestionRelation;
using LW.Shared.SeedWork;

namespace LW.Services.QuizQuestionRelationServices;

public interface IQuizQuestionRelationService
{
    Task<ApiResult<bool>> CreateQuizQuestionRelationByQuizCustom(QuizQuestionRelationCustomCreateDto quizQuestionRelationCustomCreateDto);
    Task<ApiResult<bool>> CreateQuizQuestionRelation(QuizQuestionRelationCreateDto quizQuestionRelationCreateDto);
    Task<ApiResult<bool>> UpdateQuizQuestionRelation(QuizQuestionRelationUpdateDto quizQuestionRelationUpdateDto);
    Task<ApiResult<bool>> DeleteQuizQuestionRelation(int id);
    Task<ApiResult<bool>> DeleteRangeQuizQuestionRelation(IEnumerable<int> ids);
}