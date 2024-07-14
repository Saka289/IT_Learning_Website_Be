using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.TagRepositories;

public class TagRepository : RepositoryBase<Tag, int>, ITagRepository
{
    public TagRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task CreateTag(Tag tag)
    {
        await CreateAsync(tag);
    }

    public async Task UpdateTag(Tag tag)
    {
        await UpdateAsync(tag);
    }

    public async Task<bool> DeleteTag(int id)
    {
        var tag = await GetTagById(id);
        if (tag == null)
        {
            return false;
        }

        await DeleteAsync(tag);
        return true;
    }

    public async Task<Tag> GetTagById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Tag>> GetAllTag()
    {
        return await FindAll().ToListAsync();
    }

    public  Task<IQueryable<Tag>> GetAllTagPagination()
    {
        var result = FindAll();
        return Task.FromResult(result);    }
}