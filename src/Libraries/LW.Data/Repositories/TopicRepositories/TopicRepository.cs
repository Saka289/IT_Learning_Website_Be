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

    public async Task<bool> UpdateRangeTopic(IEnumerable<Topic> topics)
    {
        topics = topics.Where(t => t != null);
        if (!topics.Any())
        {
            return false;
        }

        await UpdateListAsync(topics);
        return true;
    }

    public async Task<bool> DeleteTopic(int id)
    {
        var topic = await GetByIdAsync(id);
        if (topic != null)
        {
            await DeleteAsync(topic);
            return true;
        }

        return false;
    }

    public async Task<Topic> GetTopicById(int id)
    {
        return await FindByCondition(x => x.Id == id)
            .Include(c => c.ChildTopics)
            .ThenInclude(d => d.Document)
            .Include(d => d.Document)
            .FirstOrDefaultAsync();
    }

    public async Task<Topic> GetTopicByAllId(int id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<IEnumerable<Topic>> GetAllTopic()
    {
        return await FindAll()
            .Include(d => d.Document)
            .Include(c => c.ChildTopics)
            .Where(t => t.ParentId == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<Topic>> GetAllTopicByDocument(int id)
    {
        return await FindAll()
            .Include(d => d.Document)
            .Include(c => c.ChildTopics)
            .Where(x => x.DocumentId == id && x.ParentId == null).ToListAsync();
    }

    public Task<IQueryable<Topic>> GetAllTopicPagination()
    {
        var result = FindAll()
            .Where(t => t.ParentId == null);
        return Task.FromResult(result);
    }

    public async Task<Topic> GetAllTopicIndex(int id)
    {
        return await FindAll()
            .Include(d => d.Document).Where(d => d.Document.IsActive == true)
            .Include(c => c.ChildTopics.Where(c => c.IsActive == true))
            .ThenInclude(l => l.Lessons.Where(l => l.IsActive))
            .Include(p => p.ParentTopic)
            .ThenInclude(l => l.Lessons.Where(l => l.IsActive))
            .Include(l => l.Lessons.Where(l => l.IsActive))
            .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
    }
}