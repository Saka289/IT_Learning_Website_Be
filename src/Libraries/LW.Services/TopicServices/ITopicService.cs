﻿using LW.Shared.DTOs.Level;
using LW.Shared.DTOs.Topic;
using LW.Shared.SeedWork;

namespace LW.Services.TopicServices;

public interface ITopicService
{
    public Task<ApiResult<bool>> Create(TopicCreateDto model);
    public Task<ApiResult<bool>> Update(TopicUpdateDto model);
    public Task<ApiResult<bool>> UpdateStatus(int id);
    public Task<ApiResult<bool>> Delete(int id);
    public Task<ApiResult<IEnumerable<TopicDto>>> GetAll();
    public Task<ApiResult<TopicDto>> GetById(int id);
}