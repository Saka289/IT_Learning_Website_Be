using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.CompetitionRepositories;

public interface ICompetitionRepository : IRepositoryBase<Competition, int>
{
    Task<IEnumerable<Competition>> GetAllCompetition();
    Task<IQueryable<Competition>> GetAllCompetitionPagination();
    Task<Competition> GetCompetitionById(int id);
    Task<Competition> CreateCompetition(Competition competition);
    Task<Competition> UpdateCompetition(Competition competition);
    Task<bool> DeleteCompetition(int id);
}