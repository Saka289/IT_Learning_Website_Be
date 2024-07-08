using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.ExamImageRepositories;

public class ExamImageRepository : RepositoryBase<ExamImage, int>, IExamImageRepository
{
    public ExamImageRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<ExamImage>> GetAllExamImage()
    {
        return await FindAll().ToListAsync();
    }

    public async Task<IEnumerable<ExamImage>> GetAllExamImageByExamId(int examId)
    {
        return await FindByCondition(x=>x.ExamId==examId).ToListAsync();
    }

    public async Task<ExamImage> GetExamImageById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> CreateRangeExamImage(IEnumerable<ExamImage> examImages)
    {
        await CreateListAsync(examImages);
        return true;
    }

    public async Task<ExamImage> CreateExamImage(ExamImage e)
    {
        await CreateAsync(e);
        return e;
    }

    public async Task<ExamImage> UpdateExamImage(ExamImage e)
    {
        await UpdateAsync(e);
        return e;
    }

    public async Task<bool> DeleteExamImage(int id)
    {
        var examImage = await GetExamImageById(id);
        if (examImage == null)
        {
            return false;
        }

        await DeleteAsync(examImage);
        return true;
    }

    public async Task<bool> DeleteRangeExamImage(IEnumerable<ExamImage> examImages)
    {
        await DeleteListAsync(examImages);
        return true;
    }
}