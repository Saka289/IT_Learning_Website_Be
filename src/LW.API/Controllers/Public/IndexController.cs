using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.Services.IndexServices;
using LW.Shared.DTOs.Index;
using LW.Shared.DTOs.Index;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly IIndexService _indexService;

        public IndexController(IIndexService indexService)
        {
            _indexService = indexService;
        }

        [HttpGet("GetAllDocumentIndex/{documentId}")]
        public async Task<ActionResult<ApiResult<DocumentIndexByDocumentDto>>> GetAllDocumentIndex(int documentId)
        {
            var result = await _indexService.GetAllDocumentIndex(documentId);
            if (!result.IsSucceeded)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("GetAllTopicIndex/{topicId}")]
        public async Task<IActionResult> GetAllTopicIndex(int topicId)
        {
            var result = await _indexService.CheckTopicById(topicId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            if (result.Data == ETopicIndex.ParentTopic)
            {
                var topicParent = await _indexService.GetAllTopicParentIndex(topicId);
                return Ok(topicParent);
            }

            var topic = await _indexService.GetAllTopicIndex(topicId);
            return Ok(topic);
        }

        [HttpGet("GetAllLessonIndex/{lessonId}")]
        public async Task<IActionResult> GetAllLessonIndex(int lessonId)
        {
            var result = await _indexService.CheckLessonById(lessonId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            if (result.Data == ELessonIndex.ParentTopic)
            {
                var topicParent = await _indexService.GetAllLessonParentTopicIndex(lessonId);
                return Ok(topicParent);
            }

            var topic = await _indexService.GetAllLessonIndex(lessonId);
            return Ok(topic);
        }
    }
}