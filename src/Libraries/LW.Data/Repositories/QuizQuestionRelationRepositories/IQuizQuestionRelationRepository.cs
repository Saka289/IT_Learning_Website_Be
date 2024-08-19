using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.QuizQuestionRelationRepositories;

public interface IQuizQuestionRelationRepository :  IRepositoryBase<QuizQuestionRelation, int>
{
    Task<IEnumerable<QuizQuestionRelation>> GetAllQuizQuestionRelationByQuizId(int quizId);
    Task<QuizQuestionRelation> CreateQuizQuestionRelation(QuizQuestionRelation quizQuestionRelation);
    Task<QuizQuestionRelation> UpdateQUizQuestionRelation(QuizQuestionRelation quizQuestionRelation);
    Task<QuizQuestionRelation?> GetQuizQuestionRelationById(int id);
    Task<bool> CreateRangeQuizQuestionRelation(IEnumerable<QuizQuestionRelation> quizQuestionRelations);
    Task<bool> UpdateRangeQuizQuestionRelation(IEnumerable<QuizQuestionRelation> quizQuestionRelations);
    Task<bool> DeleteQuizQuestionRelation(int id);
    Task<bool> DeleteRangeQuizQuestionRelation(IEnumerable<QuizQuestionRelation> quizQuestionRelations);
}