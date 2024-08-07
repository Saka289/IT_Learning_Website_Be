using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.GradeRepositories;

public interface IGradeRepository : IRepositoryBase<Grade, int>
{
    Task<IEnumerable<Grade>> GetAllGrade();
    Task<IEnumerable<Grade>> GetAllGradePagination();
    Task<Grade?> GetGradeById(int id, bool isInclude);
    Task<Grade> CreateGrade(Grade grade);
    Task<Grade> UpdateGrade(Grade grade);
    Task<bool> DeleteGrade(int id);
}
