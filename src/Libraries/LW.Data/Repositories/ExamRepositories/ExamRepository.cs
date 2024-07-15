using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.ExamRepositories;

public class ExamRepository : RepositoryBase<Exam, int>, IExamRepository
{
    public ExamRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Exam>> GetAllExam()
    {
        return await FindAll().ToListAsync();
    }

    public Task<IQueryable<Exam>> GetAllExamByPagination()
    {
        var result = FindAll();
        return Task.FromResult(result);
    }

    public async Task<Exam> GetExamById(int id)
    {
        return await FindByCondition(x => x.Id == id).Include(x=>x.ExamImages).FirstOrDefaultAsync();
    }

    public async Task<Exam> CreateExam(Exam e)
    {
        await CreateAsync(e);
        return e;
    }

    public async Task<Exam> UpdateExam(Exam e)
    {
        await UpdateAsync(e);
        return e;
    }

    public async  Task<bool> DeleteExam(int id)
    {
        var exam = await GetByIdAsync(id);
        if (exam == null)
        {
            return false;
        }
        await DeleteAsync(exam);
        return true;
    }
}