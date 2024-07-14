using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.ExamAnswerRepositories;

public class ExamAnswerRepository : RepositoryBase<ExamAnswer, int>, IExamAnswerRepository
{
    public ExamAnswerRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<ExamAnswer>> GetAllExamAnswer()
    {
        return await FindAll().ToListAsync();
    }

    public async Task<IEnumerable<ExamAnswer>> GetAllExamAnswerByExamId(int examId)
    {
        return await FindByCondition(x => x.ExamId == examId).ToListAsync();
    }

    public async Task<ExamAnswer> GetExamAnswerById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<ExamAnswer> GetExamAnswerByNumberOfQuestion(int numberOfQuestion)
    {
        return await FindByCondition(x => x.NumberOfQuestion == numberOfQuestion).FirstOrDefaultAsync();
    }

    public async Task<bool> CreateRangeExamAnswer(IEnumerable<ExamAnswer> examAnswers)
    {
        await CreateListAsync(examAnswers);
        return true;
    }

    public async Task<ExamAnswer> CreateExamAnswer(ExamAnswer examAnswer)
    {
        await CreateAsync(examAnswer);
        return examAnswer;
    }

    public async Task<ExamAnswer> UpdateExamAnswer(ExamAnswer examAnswer)
    {
        await UpdateAsync(examAnswer);
        return examAnswer;
    }

    public async Task<bool> DeleteExamAnswer(int id)
    {
        var answer = await GetExamAnswerById(id);
        if (answer == null)
        {
            return false;
        }

        await DeleteAsync(answer);
        return true;
    }

    public async Task<bool> DeleteRangeExamAnswer(IEnumerable<ExamAnswer> examAnswers)
    {
        await DeleteListAsync(examAnswers);
        return true;
    }
}