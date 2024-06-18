using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.TopicRepositories;
using LW.Shared.DTOs.Topic;
using LW.Shared.SeedWork;

namespace LW.Services.TopicServices;

public class TopicService : ITopicService
{
    private readonly ITopicRepository _topicRepository;

    private readonly IMapper _mapper;

    //private readonly IDocumentService _documentService;

    public TopicService(ITopicRepository topicRepository, IMapper mapper)
    {
        _topicRepository = topicRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<bool>> Create(TopicCreateDto model)
    {
        // var documentEntity = await _documentService.GetById(model.DocumentId);
        // if (documentEntity == null)
        // {
        //     return new ApiResult<bool>(false, "Document of topic not found !!!");
        // }
        var topic = _mapper.Map<Topic>(model);
        topic.KeyWord = topic.Title.ToLower();
        await _topicRepository.CreateTopic(topic);
        return new ApiResult<bool>(true, "Create topic successfully");
    }

    public async Task<ApiResult<bool>> Update(TopicUpdateDto model)
    {
        // var documentEntity = await _documentService.GetById(model.DocumentId);
        // if (documentEntity == null)
        // {
        //     return new ApiResult<bool>(false, "Document of topic not found !!!");
        // }
        var topicEntity = await _topicRepository.GetTopicById(model.Id);
        if (topicEntity == null)
        {
            return new ApiResult<bool>(false, "Topic is not found !!!");
        }
        var topicUpdate = _mapper.Map(model, topicEntity);
        topicUpdate.KeyWord = topicUpdate.Title.ToLower();
        await _topicRepository.UpdateAsync(topicUpdate);
        return new ApiResult<bool>(true, "Update topic successfully");
    }

    public async Task<ApiResult<bool>> UpdateStatus(int id)
    {
        var obj = await _topicRepository.GetTopicById(id);
        if (obj == null)
        {
            return new ApiResult<bool>(false, "Not found !");
        }

        obj.IsActive = !obj.IsActive;
        await _topicRepository.UpdateAsync(obj);
        return new ApiResult<bool>(true, "Update status of topic successfully");
    }

    public async Task<ApiResult<bool>> Delete(int id)
    {
        var topic = await _topicRepository.GetTopicById(id);
        if (topic == null)
        {
            return new ApiResult<bool>(false, "Not found !");
        }

        var isDeleted = await _topicRepository.DeleteTopic(id);
        if (!isDeleted)
        {
            return new ApiResult<bool>(false, "Delete topic failed");
        }

        return new ApiResult<bool>(true, "Delete topic successfully");
    }

    public async Task<ApiResult<IEnumerable<TopicDto>>> GetAll()
    {
        var list = await _topicRepository.GetAllTopic();
        var result = _mapper.Map<IEnumerable<TopicDto>>(list);
        return new ApiResult<IEnumerable<TopicDto>>(true, result, "Get all topic successfully !");
    }

    public async Task<ApiResult<TopicDto>> GetById(int id)
    {
        var obj = await _topicRepository.GetByIdAsync(id);
        if (obj == null)
        {
            return new ApiResult<TopicDto>(false, "Not found !");
        }

        var result = _mapper.Map<TopicDto>(obj);
        return new ApiResult<TopicDto>(false, result, "Get topic by id successfully !");
    }
}