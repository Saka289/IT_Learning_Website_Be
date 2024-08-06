using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.EditorialRepositories;

public class EditorialRepository : RepositoryBase<Editorial, int>, IEditorialRepository
{
    public EditorialRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Editorial>> GetAllEditorial()
    {
        return await FindAll().ToListAsync();
    }

    public Task<IQueryable<Editorial>> GetAllEditorialPagination()
    {
        var result = FindAll().AsQueryable();
        return Task.FromResult(result);
    }

    public async Task<Editorial?> GetAllEditorialByProblemId(int problemId)
    {
        return await FindByCondition(e => e.ProblemId == problemId)
            .FirstOrDefaultAsync();
    }

    public async Task<Editorial?> GetEditorialById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Editorial> CreateEditorial(Editorial editorial)
    {
        await CreateAsync(editorial);
        return editorial;
    }

    public async Task<Editorial> UpdateEditorial(Editorial editorial)
    {
        await UpdateAsync(editorial);
        return editorial;
    }

    public async Task<bool> DeleteEditorial(int id)
    {
        var editorial = await GetByIdAsync(id);
        if (editorial == null)
        {
            return false;
        }

        await DeleteAsync(editorial);
        return true;
    }
}