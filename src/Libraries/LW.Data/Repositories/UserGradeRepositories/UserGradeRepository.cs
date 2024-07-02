using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.UserGradeRepositories;

public class UserGradeRepository : RepositoryBase<UserGrade, int>, IUserGradeRepository
{
    public UserGradeRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }
    public Task CreateUserGrade(UserGrade userGrade)
    {
        return CreateAsync(userGrade);
    }

    public Task CreateRangeUserGrade(IEnumerable<UserGrade> userGrades)
    {
        return CreateListAsync(userGrades);
    }

    public Task UpdateUserGrade(UserGrade userGrade)
    {
        return UpdateAsync(userGrade);
    }

    public async Task<bool> DeleteUserGrade(int id)
    {
        var userGrade = await GetUserGradeById(id);
        if (userGrade != null)
        {
            await DeleteAsync(userGrade);
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteRangeUserGrade(IEnumerable<UserGrade> userGrades)
    {
        if (userGrades != null)
        {
            await DeleteListAsync(userGrades);
            return true;
        }
        return false;
    }

    public async Task<UserGrade> GetUserGradeById(int id)
    {
        return await FindByCondition(x => x.Id == id).Include(x => x.Grade).Include(x => x.ApplicationUser)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserGrade>> GetAllUserGrade()
    {
        return await FindAll().Include(x => x.Grade).Include(x => x.ApplicationUser).ToListAsync();
    }
}