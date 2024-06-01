using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.GradeRepositories;

public class GradeRepository : RepositoryBase<Grade, int>, IGradeRepository
{
    public GradeRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Grade>> GetAllGrade()
    {
        return await FindAll().ToListAsync();
    }

    public async Task<Grade> GetGradeById(int id)
    {
        return await GetByIdAsync(id);
    }

    public Task<Grade> CreateGrade(Grade grade)
    {
        Create(grade);
        return Task.FromResult(grade);
    }

    public Task<Grade> UpdateGrade(Grade grade)
    {
        Update(grade);
        return Task.FromResult(grade);
    }

    public async Task<bool> DeleteGrade(int id)
    {
        var grade = await GetByIdAsync(id);
        if (grade == null)
        {
            return false;
        }

        await DeleteAsync(grade);
        return true;
    }
}