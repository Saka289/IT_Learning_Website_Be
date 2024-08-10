using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.GradeRepositories;

public interface IGradeRepository : IRepositoryBase<Grade, int>
{
    Task<IEnumerable<Grade>> GetAllGrade(bool isInclude = false);
    Task<IEnumerable<Grade>> GetAllGradePagination(bool isInclude = false);
    Task<Grade?> GetGradeById(int id, bool isInclude);
    Task<Grade> CreateGrade(Grade grade);
    Task<Grade> UpdateGrade(Grade grade);
    Task<bool> DeleteGrade(int id);
}
