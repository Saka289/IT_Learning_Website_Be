using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.TopicValidator;
using LW.Services.TopicServices;
using LW.Shared.DTOs.Level;
using LW.Shared.DTOs.Topic;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet("GetAllTopic")]
        public async Task<ActionResult<ApiResult<TopicDto>>> GetAllTopic()
        {
            var result = await _topicService.GetAll();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("GetAllTopicPagination")]
        public async Task<ActionResult<ApiResult<PagedList<TopicDto>>>> GetAllTopicPagination([FromQuery]PagingRequestParameters pagingRequestParameters)
        {
            var result = await _topicService.GetAllTopicPagination(pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }
        [HttpGet("GetTopicById")]
        public async Task<ActionResult<ApiResult<TopicDto>>> GetTopicById(int id)
        {
            var result = await _topicService.GetById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("SearchByTopicPagination")]
        public async Task<ActionResult<ApiResult<PagedList<TopicDto>>>> SearchByTopicPagination([FromQuery] SearchTopicDto searchTopicDto)
        {
            var result = await _topicService.SearchTopicPagination(searchTopicDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpPost("CreateTopic")]
        public async Task<ActionResult<ApiResult<bool>>> CreateTopic([FromBody] TopicCreateDto model)
        {
            var validationResult = await new CreateTopicCommandValidator().ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            var result = await _topicService.Create(model);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("UpdateTopic")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateTopic([FromBody] TopicUpdateDto model)
        {
            var validationResult = await new UpdateTopicCommandValidator().ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            var result = await _topicService.Update(model);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("UpdateStatusTopic/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusTopic(int id)
        {
            var result = await _topicService.UpdateStatus(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpDelete("DeleteTopic/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteTopic(int id)
        {
            var result = await _topicService.Delete(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}