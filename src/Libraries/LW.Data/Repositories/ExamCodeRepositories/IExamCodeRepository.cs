using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.ExamCodeRepositories;

public interface IExamCodeRepository : IRepositoryBase<ExamCode, int>
{
    Task<IEnumerable<ExamCode>> GetAllExamCode();
    Task<IEnumerable<ExamCode?>> GetAllExamCodeByExamId(int examId);
    Task<ExamCode?> GetExamCodeById(int id);
    Task<ExamCode?> GetExamCodeByCode(int examId, string code);
    Task<IEnumerable<ExamCode>> CreateRangeExamCode(IEnumerable<ExamCode> examCodes);
    Task<ExamCode> CreateExamCode(ExamCode examCode);
    Task<ExamCode> UpdateExamCode(ExamCode examCode);
    Task<bool> DeleteExamCode(int id);
}