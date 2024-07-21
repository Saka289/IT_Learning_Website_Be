using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.QuizQuestionRelationRepositories;

public class QuizQuestionRelationRepository : RepositoryBase<QuizQuestionRelation, int>, IQuizQuestionRelationRepository
{
    public QuizQuestionRelationRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<QuizQuestionRelation>> GetAllQuizQuestionRelationByQuizId(int quizId)
    {
        return await FindAll().Where(q => q.QuizId == quizId).ToListAsync();
    }

    public async Task<QuizQuestionRelation> CreateQuizQuestionRelation(QuizQuestionRelation quizQuestionRelation)
    {
        await CreateAsync(quizQuestionRelation);
        return quizQuestionRelation;
    }

    public async Task<QuizQuestionRelation> UpdateQUizQuestionRelation(QuizQuestionRelation quizQuestionRelation)
    {
        await UpdateAsync(quizQuestionRelation);
        return quizQuestionRelation;
    }

    public async Task<QuizQuestionRelation?> GetQuizQuestionRelationById(int id)
    {
        return await FindByCondition(q => q.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> CreateRangeQuizQuestionRelation(IEnumerable<QuizQuestionRelation> quizQuestionRelations)
    {
        quizQuestionRelations = quizQuestionRelations.Where(q => q != null);
        if (!quizQuestionRelations.Any())
        {
            return false;
        }

        await CreateListAsync(quizQuestionRelations);
        return true;
    }

    public async Task<bool> UpdateRangeQuizQuestionRelation(IEnumerable<QuizQuestionRelation> quizQuestionRelations)
    {
        quizQuestionRelations = quizQuestionRelations.Where(q => q != null);
        if (!quizQuestionRelations.Any())
        {
            return false;
        }

        await UpdateListAsync(quizQuestionRelations);
        return true;
    }

    public async Task<bool> DeleteQuizQuestionRelation(int id)
    {
        var quizQuestionRelation = await GetByIdAsync(id);
        if (quizQuestionRelation == null)
        {
            return false;
        }

        await DeleteAsync(quizQuestionRelation);
        return true;
    }

    public async Task<bool> DeleteRangeQuizQuestionRelation(IEnumerable<QuizQuestionRelation> quizQuestionRelations)
    {
        quizQuestionRelations = quizQuestionRelations.Where(q => q != null);
        if (!quizQuestionRelations.Any())
        {
            return false;
        }

        await DeleteListAsync(quizQuestionRelations);
        return true;
    }
}