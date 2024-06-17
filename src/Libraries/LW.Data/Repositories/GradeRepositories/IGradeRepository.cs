using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.GradeRepositories;

public interface IGradeRepository : IRepositoryBase<Grade, int>
{
    Task<IEnumerable<Grade>> GetAllGrade();
    Task<Grade> GetGradeById(int id);
    Task<Grade> CreateGrade(Grade grade);
    Task<Grade> UpdateGrade(Grade grade);
    Task<bool> DeleteGrade(int id);
}
