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
    public Task<Topic> GetTopicById(int id)
    {
        return GetByIdAsync(id);
    }

    public async Task<IEnumerable<Topic>> GetAllTopic()
    {
        return await FindAll().ToListAsync();
    }
}