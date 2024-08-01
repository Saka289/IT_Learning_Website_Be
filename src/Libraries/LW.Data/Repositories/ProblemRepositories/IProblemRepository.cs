using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.ProblemRepositories;

public interface IProblemRepository : IRepositoryBase<Problem, int>
{
    Task<IEnumerable<Problem>> GetAllProblem();
    Task<Problem?> GetProblemById(int id);
    Task<Problem> CreateProblem(Problem problem);
    Task<Problem> UpdateProblem(Problem problem);
    Task<bool> DeleteProblem(int id);
}