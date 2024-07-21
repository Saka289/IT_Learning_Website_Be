using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.TopicRepositories;

public interface ITopicRepository : IRepositoryBase<Topic, int>
{
    Task CreateTopic(Topic topic);
    Task UpdateTopic(Topic topic);
    Task<bool> UpdateRangeTopic(IEnumerable<Topic> topics);
    Task<bool> DeleteTopic(int id);
    Task<Topic> GetTopicById(int id);
    Task<Topic> GetTopicByAllId(int id);
    Task<IEnumerable<Topic>> GetAllTopic();
    Task<IEnumerable<Topic>> GetAllTopicByDocument(int id);
    Task<IEnumerable<Topic>> GetAllTopicChildByParentId(int parentId);

    Task<IQueryable<Topic>> GetAllTopicPagination();
    Task<Topic> GetAllTopicIndex(int id);
}