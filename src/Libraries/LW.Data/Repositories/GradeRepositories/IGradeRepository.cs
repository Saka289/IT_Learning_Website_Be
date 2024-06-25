using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.GradeRepositories;

public interface IGradeRepository : IRepositoryBase<Grade, int>
{
    Task<IEnumerable<Grade>> GetAllGrade();
    Task<IEnumerable<Grade>> GetAllGradeByLevel(int id);
    Task<IQueryable<Grade>> GetAllGradePagination();
    Task<Grade> GetGradeById(int id);
    Task<Grade> CreateGrade(Grade grade);
    Task<Grade> UpdateGrade(Grade grade);
    Task<bool> DeleteGrade(int id);
}
