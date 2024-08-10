using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.CompetitionRepositories;

public class CompetitionRepository : RepositoryBase<Competition, int>, ICompetitionRepository
{
    public CompetitionRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Competition>> GetAllCompetition()
    {
        return await FindAll().ToListAsync();
    }

    public async Task<Competition> GetCompetitionById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Competition> CreateCompetition(Competition competition)
    {
        await CreateAsync(competition);
        return competition;
    }

    public async Task<Competition> UpdateCompetition(Competition competition)
    {
        await UpdateAsync(competition);
        return competition;
    }

    public async Task<bool> DeleteCompetition(int id)
    {
        var competition = await GetCompetitionById(id);
        if (competition == null)
        {
            return false;
        }

        await DeleteAsync(competition);
        return true;
    }
}