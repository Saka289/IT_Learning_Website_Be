using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.ProgramLanguageRepositories;

public class ProgramLanguageRepository : RepositoryBase<ProgramLanguage, int>, IProgramLanguageRepository
{
    public ProgramLanguageRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<ProgramLanguage>> GetAllProgramLanguage()
    {
        return await FindAll().ToListAsync();
    }

    public Task<IQueryable<ProgramLanguage>> GetAllProgramLanguagePagination()
    {
        var result = FindAll().AsQueryable();
        return Task.FromResult(result);
    }

    public async Task<ProgramLanguage?> GetProgramLanguageById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<ProgramLanguage> CreateProgramLanguage(ProgramLanguage programLanguage)
    {
        await CreateAsync(programLanguage);
        return programLanguage;
    }

    public async Task<ProgramLanguage> UpdateProgramLanguage(ProgramLanguage programLanguage)
    {
        await UpdateAsync(programLanguage);
        return programLanguage;
    }

    public async Task<bool> DeleteProgramLanguage(int id)
    {
        var programLanguage = await GetByIdAsync(id);
        if (programLanguage == null)
        {
            return false;
        }

        await DeleteAsync(programLanguage);
        return true;
    }
}