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
            .Include(l => l.Lesson)
            .Include(l => l.Topic)
            .Include(q => q.QuizQuestionRelations)
            .ToListAsync();
    }

    public async Task<IEnumerable<Quiz>> GetAllQuizPagination()
    {
        var result = await FindAll()
            .Include(l => l.Lesson)
            .Include(l => l.Topic)
            .Include(q => q.QuizQuestionRelations)
            .ToListAsync();
        return result;
    }

    public Task<IQueryable<Quiz>> GetAllQuizByTopicIdPagination(int topicId)
    {
        var result = FindAll()
            .Include(l => l.Lesson)
            .Include(l => l.Topic)
            .Include(q => q.QuizQuestionRelations)
            .Where(q => q.TopicId == topicId).AsQueryable();
        return Task.FromResult(result);
    }

    public Task<IQueryable<Quiz>> GetAllQuizByLessonIdPagination(int lessonId)
    {
        var result = FindAll()
            .Include(l => l.Lesson)
            .Include(l => l.Topic)
            .Include(q => q.QuizQuestionRelations)
            .Where(q => q.LessonId == lessonId).AsQueryable();
        return Task.FromResult(result);
    }

    public async Task<Quiz?> GetQuizById(int id)
    {
        return await FindByCondition(q => q.Id == id)
            .Include(l => l.Lesson)
            .Include(l => l.Topic)
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
}