﻿using LW.Shared.DTOs.Tag;
using LW.Shared.DTOs.Topic;
using LW.Shared.SeedWork;

namespace LW.Services.TopicServices;

public interface ITopicService
{
    public Task<ApiResult<IEnumerable<TopicDto>>> GetAll(bool? status);
    public Task<ApiResult<IEnumerable<TopicDto>>> GetAllTopicByDocument(int id, bool? status);
    public Task<ApiResult<IEnumerable<TagDto>>> GetTopicIdByTag(int id);
    public Task<ApiResult<PagedList<TopicDto>>> GetAllTopicPagination(SearchTopicDto searchTopicDto);
    public Task<ApiResult<TopicDto>> GetById(int id);
    public Task<ApiResult<bool>> Create(TopicCreateDto model);
    public Task<ApiResult<bool>> Update(TopicUpdateDto model);
    public Task<ApiResult<bool>> UpdateStatus(int id);
    public Task<ApiResult<bool>> Delete(int id);
}