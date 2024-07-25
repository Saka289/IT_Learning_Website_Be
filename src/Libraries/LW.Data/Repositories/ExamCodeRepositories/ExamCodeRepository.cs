using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.ExamCodeRepositories;

public class ExamCodeRepository : RepositoryBase<ExamCode, int>, IExamCodeRepository
{
    public ExamCodeRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<ExamCode>> GetAllExamCode()
    {
        return await FindAll().Include(x=>x.Exam).ToListAsync();
    }

    public async Task<IEnumerable<ExamCode>> GetAllExamCodeByExamId(int examId)
    {
        return await FindByCondition(x => x.ExamId == examId).Include(x=>x.Exam).ToListAsync();
    }

    public async Task<ExamCode> GetExamCodeById(int id)
    {
        return await FindByCondition(x => x.Id == id).Include(x=>x.Exam).FirstOrDefaultAsync();
    }

    public async Task<ExamCode> GetExamCodeByCode(string code)
    {
        return await FindByCondition(x => x.Code == code).Include(x=>x.Exam).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ExamCode>> CreateRangeExamCode(IEnumerable<ExamCode> ExamCodes)
    {
        await CreateListAsync(ExamCodes);
        return ExamCodes;
    }

    public async Task<ExamCode> CreateExamCode(ExamCode ExamCode)
    {
        await CreateAsync(ExamCode);
        return ExamCode;
    }

    public async Task<ExamCode> UpdateExamCode(ExamCode ExamCode)
    {
        await UpdateAsync(ExamCode);
        return ExamCode;
    }

    public async Task<bool> DeleteExamCode(int id)
    {
        var examCode = await GetExamCodeById(id);
        if (examCode == null)
        {
            return false;
        }

        await DeleteAsync(examCode);
        return true;
    }
}