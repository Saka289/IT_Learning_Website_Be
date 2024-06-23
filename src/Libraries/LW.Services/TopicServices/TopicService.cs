﻿using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Infrastructure.Extensions;
using LW.Services.DocumentService;
using LW.Shared.Constant;
using LW.Shared.DTOs.Topic;
using LW.Shared.SeedWork;

namespace LW.Services.TopicServices;

public class TopicService : ITopicService
{
    private readonly ITopicRepository _topicRepository;

    private readonly IMapper _mapper;

    private readonly IDocumentRepository _documentRepository;
    private readonly IElasticSearchService<Topic, int> _elasticSearchService;

    public TopicService(ITopicRepository topicRepository, IMapper mapper, IDocumentRepository documentRepository, IElasticSearchService<Topic, int> elasticSearchService)
    {
        _topicRepository = topicRepository;
        _mapper = mapper;
        _documentRepository = documentRepository;
        _elasticSearchService = elasticSearchService;
    }

    public async Task<ApiResult<bool>> Create(TopicCreateDto model)
    {
        var documentEntity = await _documentRepository.GetDocumentById(model.DocumentId);
        if (documentEntity==null)
        {
            return new ApiResult<bool>(false, "Document of topic not found !!!");
        }
        var topic = _mapper.Map<Topic>(model);
        topic.KeyWord = model.Title.RemoveDiacritics();
        await _topicRepository.CreateTopic(topic);
        await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticTopics, topic, g => g.Id);
        return new ApiResult<bool>(true, "Create topic successfully");
    }

    public async Task<ApiResult<bool>> Update(TopicUpdateDto model)
    {
        var documentEntity = await _documentRepository.GetDocumentById(model.DocumentId);
        if (documentEntity == null)
        {
            return new ApiResult<bool>(false, "Document of topic not found !!!");
        }
        var topicEntity = await _topicRepository.GetTopicById(model.Id);
        if (topicEntity == null)
        {
            return new ApiResult<bool>(false, "Topic is not found !!!");
        }
        var topicUpdate = _mapper.Map(model, topicEntity);
        topicUpdate.KeyWord = model.Title.RemoveDiacritics();
        await _topicRepository.UpdateAsync(topicUpdate);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticTopics, topicUpdate, model.Id);
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
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticTopics, obj, id);
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
        await _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticTopics, id);
        return new ApiResult<bool>(true, "Delete topic successfully");
    }

    public async Task<ApiResult<IEnumerable<TopicDto>>> GetAll()
    {
        var list = await _topicRepository.GetAllTopic();
        var result = _mapper.Map<IEnumerable<TopicDto>>(list);
        return new ApiResult<IEnumerable<TopicDto>>(true, result, "Get all topic successfully !");
    }
    
    public async Task<ApiResult<PagedList<TopicDto>>> GetAllTopicPagination(PagingRequestParameters pagingRequestParameters)
    {
        var topics = await _topicRepository.GetAllTopicPagination();
        if (topics == null)
        {
            return new ApiResult<PagedList<TopicDto>>(false, "Grade is null !!!");
        }
        var result = _mapper.ProjectTo<TopicDto>(topics);
        var pagedResult = await PagedList<TopicDto>.ToPageList(result, pagingRequestParameters.PageIndex, pagingRequestParameters.PageSize);

        return new ApiSuccessResult<PagedList<TopicDto>>(pagedResult);
    }
    

    public async Task<ApiResult<IEnumerable<TopicDto>>> SearchTopic(SearchTopicDto searchTopicDto)
    {
        var topics = await _elasticSearchService.SearchDocumentAsync(ElasticConstant.ElasticTopics, searchTopicDto);
        if (topics is null)
        {
            return new ApiResult<IEnumerable<TopicDto>>(false, $"Topics not found by {searchTopicDto.Key} !!!");
        }

        var result = _mapper.Map<IEnumerable<TopicDto>>(topics);
        return new ApiSuccessResult<IEnumerable<TopicDto>>(result);
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