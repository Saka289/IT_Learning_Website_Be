using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.TopicRepositories;

public interface ITopicRepository : IRepositoryBase<Topic, int>
{
    Task<Topic> CreateTopic(Topic topic);
    Task<Topic> UpdateTopic(Topic topic);
    Task<bool> UpdateRangeTopic(IEnumerable<Topic> topics);
    Task<bool> DeleteTopic(int id);
    Task<Topic?> GetTopicById(int id);
    Task<Topic?> GetTopicByAllId(int id);
    Task<IEnumerable<Topic>> GetAllTopic();
    Task<IEnumerable<Topic>> GetAllTopicChild(int id);
    Task<IEnumerable<Topic>> GetAllTopicByDocument(int id);
    Task<IEnumerable<Topic>> GetAllTopicByDocumentAll(int id);
    Task<Topic> GetAllTopicIndex(int id);
}