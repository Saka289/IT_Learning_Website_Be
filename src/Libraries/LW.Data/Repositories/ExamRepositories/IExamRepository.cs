using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Shared.Enums;

namespace LW.Data.Repositories.ExamRepositories;

public interface IExamRepository : IRepositoryBase<Exam, int>
{
    Task<IEnumerable<Exam>> GetAllExam();
    Task<IQueryable<Exam>> GetAllExamByPagination();
    Task<Exam> GetExamById(int id);
    Task<IEnumerable<Exam>> GetExamByType(EExamType type);
    Task<Exam> CreateExam(Exam e);
    Task<Exam> UpdateExam(Exam e);
    Task<bool> DeleteExam(int id);
}