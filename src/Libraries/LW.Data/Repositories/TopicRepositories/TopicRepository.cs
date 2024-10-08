﻿using LW.Data.Common;
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

    public async Task<Topic> CreateTopic(Topic topic)
    {
        await CreateAsync(topic);
        return topic;
    }

    public async Task<Topic> UpdateTopic(Topic topic)
    {
        await UpdateAsync(topic);
        return topic;
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

    public async Task<Topic?> GetTopicById(int id)
    {
        return await FindByCondition(x => x.Id == id)
            .Include(c => c.ChildTopics)
            .ThenInclude(d => d.Document)
            .Include(d => d.Document)
            .FirstOrDefaultAsync();
    }

    public async Task<Topic?> GetTopicByAllId(int id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<IEnumerable<Topic>> GetAllTopic()
    {
        return await FindAll()
            .Include(d => d.Document)
            .Include(c => c.ChildTopics)
            .ThenInclude(d => d.Document)
            .Where(t => t.ParentId == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<Topic>> GetAllTopicChild(int id)
    {
        return await FindAll().Where(t => t.ParentId == id).ToListAsync();
    }

    public async Task<IEnumerable<Topic>> GetAllTopicByDocument(int id)
    {
        return await FindAll()
            .Include(d => d.Document)
            .Include(c => c.ChildTopics)
            .ThenInclude(d => d.Document)
            .Where(x => x.DocumentId == id && x.ParentId == null).ToListAsync();
    }

    public async Task<IEnumerable<Topic>> GetAllTopicByDocumentAll(int id)
    {
        return await FindAll().Where(x => x.DocumentId == id).ToListAsync();
    }

    public async Task<Topic> GetAllTopicIndex(int id)
    {
        return await FindAll()
            .Include(d => d.Document).Where(d => d.Document.IsActive)
            .Include(c => c.ChildTopics.Where(c => c.IsActive))
            .ThenInclude(l => l.Lessons.Where(l => l.IsActive).OrderBy(l => l.Index))
            .Include(p => p.ParentTopic)
            .ThenInclude(l => l.Lessons.Where(l => l.IsActive).OrderBy(l => l.Index))
            .Where(p => p.IsActive)
            .Include(l => l.Lessons.Where(l => l.IsActive).OrderBy(l => l.Index))
            .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
    }

    public async Task<IEnumerable<Topic>> SearchTopicByTag(string tag, bool order)
    {
        var result = order
            ? await FindAll()
                .Include(t => t.ChildTopics)
                .Where(t => t.KeyWord.Contains(tag) && t.ParentId == null).OrderByDescending(t => t.CreatedDate)
                .ToListAsync()
            : await FindAll()
                .Include(t => t.ChildTopics)
                .Where(t => t.KeyWord.Contains(tag) && t.ParentId == null).ToListAsync();
        return result;
    }
}