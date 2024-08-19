using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using Nest;

namespace LW.Data.Repositories.SolutionRepositories;

public interface ISolutionRepository : IRepositoryBase<Solution, int>
{
    Task<IEnumerable<Solution>> GetAllSolution();
    Task<IEnumerable<Solution>> GetAllSolutionByProblemId(int problemId, bool isInclude = false);
    Task<IQueryable<Solution>> GetAllSolutionPagination();
    Task<Solution?> GetSolutionById(int id);
    Task<Solution> CreateSolution(Solution solution);
    Task<Solution> UpdateSolution(Solution solution);
    Task<bool> DeleteSolution(int id);
}