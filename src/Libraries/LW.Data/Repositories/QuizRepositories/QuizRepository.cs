using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.QuizRepositories;

public class QuizRepository : RepositoryBase<Quiz, int>, IQuizRepository
{
    public QuizRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Quiz>> GetAllQuiz()
    {
        return await FindAll()
            .Include(l => l.Lesson).DefaultIfEmpty()
            .Include(l => l.Topic).DefaultIfEmpty()
            .Include(q => q.QuizQuestionRelations).DefaultIfEmpty()
            .ToListAsync();
    }

    public async Task<IEnumerable<Quiz>> GetAllQuizCustom()
    {
        return await FindAll().Where(q => q.TopicId == null && q.LessonId == null).ToListAsync();
    }

    public async Task<IEnumerable<Quiz>> GetAllQuizByTopicId(int topicId, bool isInclude = false)
    {
        if (!isInclude)
        {
            return await FindAll().Where(q => q.TopicId == topicId).ToListAsync();
        }

        return await FindAll()
            .Include(l => l.Lesson)
            .Include(l => l.Topic)
            .Include(q => q.QuizQuestionRelations)
            .Where(q => q.TopicId == topicId).ToListAsync();
    }

    public async Task<IEnumerable<Quiz>> GetAllQuizByLessonId(int lessonId, bool isInclude = false)
    {
        if (!isInclude)
        {
            return await FindAll().Where(q => q.LessonId == lessonId).ToListAsync();
        }

        return await FindAll()
            .Include(l => l.Lesson).DefaultIfEmpty()
            .Include(l => l.Topic).DefaultIfEmpty()
            .Include(q => q.QuizQuestionRelations).DefaultIfEmpty()
            .Where(q => q.LessonId == lessonId).ToListAsync();
    }

    public async Task<IEnumerable<Quiz>> SearchQuizByTag(string tag, bool order)
    {
        var result = order
            ? await FindAll()
                .Include(q => q.QuizQuestionRelations).DefaultIfEmpty()
                .Where(q => q.KeyWord.Contains(tag)).OrderByDescending(q => q.CreatedDate).ToListAsync()
            : await FindAll()
                .Include(q => q.QuizQuestionRelations).DefaultIfEmpty()
                .Where(q => q.KeyWord.Contains(tag)).ToListAsync();
        return result;
    }

    public async Task<Quiz?> GetQuizById(int id)
    {
        return await FindByCondition(q => q.Id == id)
            .Include(l => l.Lesson).DefaultIfEmpty()
            .Include(l => l.Topic).DefaultIfEmpty()
            .FirstOrDefaultAsync();
    }

    public async Task<Quiz> CreateQuiz(Quiz quiz)
    {
        await CreateAsync(quiz);
        return quiz;
    }

    public async Task<Quiz> UpdateQuiz(Quiz quiz)
    {
        await UpdateAsync(quiz);
        return quiz;
    }

    public async Task<bool> DeleteQuiz(int id)
    {
        var quiz = await GetByIdAsync(id);
        if (quiz == null)
        {
            return false;
        }

        await DeleteAsync(quiz);
        return true;
    }

    public async Task<bool> DeleteRangeQuiz(IEnumerable<Quiz> quizzes)
    {
        quizzes = quizzes.Where(q => q != null);
        if (!quizzes.Any())
        {
            return false;
        }

        await DeleteListAsync(quizzes);
        return true;
    }
}