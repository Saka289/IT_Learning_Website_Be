using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.ExecuteCodeRepositories;

public interface IExecuteCodeRepository : IRepositoryBase<ExecuteCode, int>
{
    Task<IEnumerable<ExecuteCode>> GetAllExecuteCode();
    Task<IEnumerable<ExecuteCode>> GetAllExecuteCodeByProblemId(int problemId);
    Task<IQueryable<ExecuteCode>> GetAllExecuteCodePagination();
    Task<ExecuteCode?> GetExecuteCodeById(int id);
    Task<ExecuteCode?> GetExecuteCodeByProblemIdLanguageId(int problemId, int languageId);
    Task<ExecuteCode> CreateExecuteCode(ExecuteCode executeCode);
    Task<ExecuteCode> UpdateExecuteCode(ExecuteCode executeCode);
    Task<bool> DeleteExecuteCode(int id);
    Task<bool> DeleteRangeExecuteCode(IEnumerable<ExecuteCode> executeCode);
}