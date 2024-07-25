using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.ExamCodeRepositories;

public interface IExamCodeRepository : IRepositoryBase<ExamCode, int>
{
    Task<IEnumerable<ExamCode>> GetAllExamCode();
    Task<IEnumerable<ExamCode>> GetAllExamCodeByExamId(int examId);
    Task<ExamCode> GetExamCodeById(int id);
    Task<ExamCode> GetExamCodeByCode(string  code);
    Task<IEnumerable<ExamCode>> CreateRangeExamCode(IEnumerable<ExamCode> ExamCodes);
    Task<ExamCode> CreateExamCode(ExamCode ExamCode);
    Task<ExamCode> UpdateExamCode(ExamCode ExamCode);
    Task<bool> DeleteExamCode(int id);
}