using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.Services.TopicServices;
using LW.Shared.DTOs.Level;
using LW.Shared.DTOs.Topic;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<ApiResult<TopicDto>>> GetAllLevel()
        {
            var result = await _topicService.GetAll();
            return Ok(result);
        }
        [HttpGet("GetTopicById")]
        public async Task<ActionResult<ApiResult<TopicDto>>> GetTopicById(int id)
        {
            var result = await _topicService.GetById(id);
            return Ok(result);
        }
        [HttpGet("SearchTopic")]
        public async Task<ActionResult<IEnumerable<TopicDto>>> SearchTopic([FromQuery] SearchTopicDto searchTopicDto)
        {
            var result = await _topicService.SearchTopic(searchTopicDto);
            return Ok(result);
        }
        [HttpPost("CreateTopic")]
        public async Task<ActionResult<ApiResult<bool>>> CreateTopic([FromBody] TopicCreateDto model)
        {
            var result = await _topicService.Create(model);
            return Ok(result);
        }
        [HttpPut("UpdateTopic")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateTopic([FromBody] TopicUpdateDto model)
        {
            var result = await _topicService.Update(model);
            return Ok(result);
        }
        [HttpPut("UpdateStatusTopic/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusTopic(int id)
        {
            var result = await _topicService.UpdateStatus(id);
            return Ok(result);
        }
        [HttpDelete("DeleteTopic/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteTopic(int id)
        {
            var result = await _topicService.Delete(id);
            return Ok(result);
        }
    }
}
