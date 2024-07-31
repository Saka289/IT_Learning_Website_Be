using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.EditorialRepositories;

public interface IEditorialRepository : IRepositoryBase<Editorial, int>
{
    Task<IEnumerable<Editorial>> GetAllEditorial();
    Task<IQueryable<Editorial>> GetAllEditorialPagination();
    Task<Editorial?> GetEditorialById(int id);
    Task<Editorial?> GetAllEditorialByProblemId(int problemId);
    Task<Editorial> CreateEditorial(Editorial editorial);
    Task<Editorial> UpdateEditorial(Editorial editorial);
    Task<bool> DeleteEditorial(int id);
}