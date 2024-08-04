using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.QuizQuestionRepositories;

public class QuizQuestionRepository : RepositoryBase<QuizQuestion, int>, IQuizQuestionRepository
{
    public QuizQuestionRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<QuizQuestion>> GetAllQuizQuestion()
    {
        return await FindAll()
            .Include(qa => qa.QuizAnswers)
            .ToListAsync();
    }

    public async Task<IEnumerable<QuizQuestion>> GetAllQuizQuestionPagination()
    {
        var result = await FindAll()
            .Include(qa => qa.QuizAnswers)
            .Include(qr => qr.QuizQuestionRelations)
            .ToListAsync();
        return result;
    }

    public async Task<IEnumerable<QuizQuestion>> GetAllQuizQuestionByQuizId(int id)
    {
        var result = await FindAll()
            .Include(qa => qa.QuizAnswers)
            .Include(qr => qr.QuizQuestionRelations)
            .Where(q => q.QuizQuestionRelations.Any(q => q.QuizId == id)).ToListAsync();
        return result;
    }

    public Task<QuizQuestion?> GetQuizQuestionById(int id)
    {
        return FindByCondition(g => g.Id == id)
            .Include(qa => qa.QuizAnswers)
            .Include(qr => qr.QuizQuestionRelations)
            .FirstOrDefaultAsync();
    }

    public async Task<QuizQuestion> CreateQuizQuestion(QuizQuestion quizQuestion)
    {
        await CreateAsync(quizQuestion);
        return quizQuestion;
    }

    public async Task<IEnumerable<QuizQuestion>> CreateRangeQuizQuestion(IEnumerable<QuizQuestion> quizQuestions)
    {
        quizQuestions = quizQuestions.Where(l => l != null);
        if (!quizQuestions.Any())
        {
            return null;
        }

        await CreateListAsync(quizQuestions);
        return quizQuestions;
    }

    public async Task<QuizQuestion> UpdateQuizQuestion(QuizQuestion quizQuestion)
    {
        await UpdateAsync(quizQuestion);
        return quizQuestion;
    }

    public async Task<bool> UpdateRangeQuizQuestion(IEnumerable<QuizQuestion> quizQuestions)
    {
        quizQuestions = quizQuestions.Where(l => l != null);
        if (!quizQuestions.Any())
        {
            return false;
        }

        await UpdateListAsync(quizQuestions);
        return true;
    }

    public async Task<bool> DeleteQuizQuestion(int id)
    {
        var quizQuestion = await GetByIdAsync(id);
        if (quizQuestion == null)
        {
            return false;
        }

        await DeleteAsync(quizQuestion);
        return true;
    }
}