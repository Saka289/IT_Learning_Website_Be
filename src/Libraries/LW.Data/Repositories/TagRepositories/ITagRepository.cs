using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.TagRepositories;

public interface ITagRepository : IRepositoryBase<Tag, int>
{
    Task CreateTag(Tag tag);
    Task UpdateTag(Tag tag);
    Task<bool> DeleteTag(int id);
    Task<Tag> GetTagById(int id);
    Task<IEnumerable<Tag>> GetAllTag();
    Task<IQueryable<Tag>> GetAllTagPagination();
    Task<Tag> GetTagByKeyword(string key);

    
}