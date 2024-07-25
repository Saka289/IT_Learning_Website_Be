using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.QuizAnswerRepositories;

public class QuizAnswerRepository : RepositoryBase<QuizAnswer, int>, IQuizAnswerRepository
{
    public QuizAnswerRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<QuizAnswer>> GetAllQuizAnswerByQuizQuestionId(int id)
    {
        return await FindAll().Where(q => q.QuizQuestionId == id).ToListAsync();
    }

    public async Task<QuizAnswer?> GetQuizAnswerByQuizQuestionId(int id)
    {
        return await FindByCondition(x => x.QuizQuestionId == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<QuizAnswer>> GetQuizAnswerByQuizIdCorrect(int quizId)
    {
        var result = await FindAll()
            .Include(q => q.QuizQuestion)
            .ThenInclude(qr => qr.QuizQuestionRelations)
            .Where(q => q.QuizQuestion.QuizQuestionRelations.Any(q => q.QuizId == quizId) && q.IsCorrect)
            .ToListAsync();
        return result;
    }

    public async Task<QuizAnswer?> GetQuizAnswerById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<QuizAnswer> CreateQuizAnswer(QuizAnswer quizAnswer)
    {
        await CreateAsync(quizAnswer);
        return quizAnswer;
    }

    public async Task<bool> CreateRangeQuizAnswer(IEnumerable<QuizAnswer> quizAnswers)
    {
        quizAnswers = quizAnswers.Where(l => l != null);
        if (!quizAnswers.Any())
        {
            return false;
        }

        await CreateListAsync(quizAnswers);
        return true;
    }

    public async Task<QuizAnswer> UpdateQuizAnswer(QuizAnswer quizAnswer)
    {
        await UpdateAsync(quizAnswer);
        return quizAnswer;
    }

    public async Task<bool> UpdateRangeQuizAnswer(IEnumerable<QuizAnswer> quizAnswers)
    {
        quizAnswers = quizAnswers.Where(l => l != null);
        if (!quizAnswers.Any())
        {
            return false;
        }

        await UpdateListAsync(quizAnswers);
        return true;
    }

    public async Task<bool> DeleteQuizAnswer(int id)
    {
        var quizAnswer = await GetByIdAsync(id);
        if (quizAnswer == null)
        {
            return false;
        }

        await DeleteAsync(quizAnswer);
        return true;
    }

    public async Task<bool> DeleteRangeAnswer(IEnumerable<QuizAnswer> quizAnswers)
    {
        quizAnswers = quizAnswers.Where(l => l != null);
        if (!quizAnswers.Any())
        {
            return false;
        }

        await DeleteListAsync(quizAnswers);
        return true;
    }
}