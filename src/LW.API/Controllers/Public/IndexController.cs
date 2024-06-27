using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.Services.IndexServices;
using LW.Shared.DTOs.Index;
using LW.Shared.DTOs.Index;
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
        public async Task<ActionResult<IEnumerable<DocumentIndexByDocumentDto>>> GetAllDocumentIndex(int documentId)
        {
            var result = await _indexService.GetAllDocumentIndex(documentId);
            if (!result.IsSucceeded)
            {
                return NotFound();
            }
            return Ok(result);
        }
        
        [HttpGet("GetAllTopicIndex/{topicId}")]
        public async Task<ActionResult<IEnumerable<DocumentIndexByDocumentDto>>> GetAllTopicIndex(int topicId)
        {
            var result = await _indexService.GetAllTopicIndex(topicId);
            if (!result.IsSucceeded)
            {
                return NotFound();
            }
            return Ok(result);
        }
        
        [HttpGet("GetAllLessonIndex/{lessonId}")]
        public async Task<ActionResult<IEnumerable<DocumentIndexByLessonDto>>> GetAllLessonIndex(int lessonId)
        {
            var result = await _indexService.GetAllLessonIndex(lessonId);
            if (!result.IsSucceeded)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
