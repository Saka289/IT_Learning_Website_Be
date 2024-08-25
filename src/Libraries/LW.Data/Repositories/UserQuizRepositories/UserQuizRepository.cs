using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.UserQuizRepositories;

public class UserQuizRepository : RepositoryBase<UserQuiz, int>, IUserQuizRepository
{
    public UserQuizRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<UserQuiz>> GetAllUserQuizByUserId(string userId)
    {
        return await FindAll()
            .Include(q => q.Quiz)
            .Where(uq => uq.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<UserQuiz>> GetAllUserQuizByQuizId(int quizId)
    {
        return await FindAll().Where(uq => uq.QuizId == quizId).ToListAsync();
    }

    public async Task<UserQuiz?> GetUserQuizById(int id)
    {
        return await FindByCondition(uq => uq.Id == id).FirstOrDefaultAsync();
    }

    public async Task<UserQuiz> CreateUserQuiz(UserQuiz userQuiz)
    {
        await CreateAsync(userQuiz);
        return await Task.FromResult(userQuiz);
    }

    public async Task<bool> CreateRangeQuiz(IEnumerable<UserQuiz> userQuizzes)
    {
        userQuizzes = userQuizzes.Where(l => l != null);
        if (!userQuizzes.Any())
        {
            return false;
        }

        await CreateListAsync(userQuizzes);
        return true;
    }

    public async Task<UserQuiz> UpdateUserQuiz(UserQuiz userQuiz)
    {
        await UpdateAsync(userQuiz);
        return await Task.FromResult(userQuiz);
    }

    public async Task<bool> UpdateRangeUserQuiz(IEnumerable<UserQuiz> userQuizzes)
    {
        userQuizzes = userQuizzes.Where(l => l != null);
        if (!userQuizzes.Any())
        {
            return false;
        }

        await UpdateListAsync(userQuizzes);
        return true;
    }

    public async Task<bool> DeleteUserQuiz(int id)
    {
        var userQuiz = await GetByIdAsync(id);
        if (userQuiz == null)
        {
            return false;
        }

        await DeleteAsync(userQuiz);
        return true;
    }

    public async Task<bool> DeleteRangeUserQuiz(IEnumerable<UserQuiz> userQuizzes)
    {
        userQuizzes = userQuizzes.Where(l => l != null);
        if (!userQuizzes.Any())
        {
            return false;
        }

        await DeleteListAsync(userQuizzes);
        return true;
    }
}