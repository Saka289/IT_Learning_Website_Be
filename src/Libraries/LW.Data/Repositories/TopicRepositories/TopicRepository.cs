using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.TopicRepositories;

public class TopicRepository : RepositoryBase<Topic, int>, ITopicRepository
{
    public TopicRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public Task CreateTopic(Topic topic)
    {
        return CreateAsync(topic);
    }

    public Task UpdateTopic(Topic topic)
    {
        return UpdateAsync(topic);
    }

    public async Task<bool> DeleteTopic(int id)
    {
        var topic = await GetTopicById(id);
        if (topic != null)
        {
            await DeleteAsync(topic);
            return true;
        }

        return false;
    }

    public async Task<Topic> GetTopicById(int id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<IEnumerable<Topic>> GetAllTopic()
    {
        return await FindAll().Include(d => d.Document).ToListAsync();
    }

    public async Task<IEnumerable<Topic>> GetAllTopicByDocument(int id)
    {
        return await FindAll().Include(d => d.Document).Where(x => x.DocumentId == id).ToListAsync();
    }

    public Task<IQueryable<Topic>> GetAllTopicPagination()
    {
        var result = FindAll();
        return Task.FromResult(result);
    }

    public async Task<Topic> GetAllTopicIndex(int id)
    {
        return await FindAll().Include(d => d.Document).Include(l => l.Lessons).FirstOrDefaultAsync(t => t.Id == id);
    }
}