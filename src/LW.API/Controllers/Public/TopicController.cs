using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.TopicValidator;
using LW.Services.TopicServices;
using LW.Shared.Constant;
using LW.Shared.DTOs.Tag;
using LW.Shared.DTOs.Topic;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleConstant.RoleAdmin},{RoleConstant.RoleContentManager}")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet("GetAllTopic")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<TopicDto>>> GetAllTopic(bool? status)
        {
            var result = await _topicService.GetAll(status);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetAllTopicByDocument/{documentId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<TopicDto>>> GetAllTopicByDocument(int documentId, bool? status)
        {
            var result = await _topicService.GetAllTopicByDocument(documentId, status);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        
        [HttpGet("GetTopicIdByTag/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<IEnumerable<TagDto>>>> GetTopicIdByTag(int id)
        {
            var result = await _topicService.GetTopicIdByTag(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        
        [HttpGet("GetAllTopicPagination")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<PagedList<TopicDto>>>> GetAllTopicPagination([FromQuery] SearchTopicDto searchTopicDto)
        {
            var result = await _topicService.GetAllTopicPagination(searchTopicDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
        
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetTopicById")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<TopicDto>>> GetTopicById(int id)
        {
            var result = await _topicService.GetById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

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