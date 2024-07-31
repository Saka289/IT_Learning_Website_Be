using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.ProgramLanguageRepositories;

public interface IProgramLanguageRepository : IRepositoryBase<ProgramLanguage, int>
{
    Task<IEnumerable<ProgramLanguage>> GetAllProgramLanguage();
    Task<IQueryable<ProgramLanguage>> GetAllProgramLanguagePagination();
    Task<ProgramLanguage?> GetProgramLanguageById(int id);
    Task<ProgramLanguage> CreateProgramLanguage(ProgramLanguage programLanguage);
    Task<ProgramLanguage> UpdateProgramLanguage(ProgramLanguage programLanguage);
    Task<bool> DeleteProgramLanguage(int id);
}