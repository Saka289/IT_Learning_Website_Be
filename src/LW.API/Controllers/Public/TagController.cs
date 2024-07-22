using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.TagValidatior;
using LW.Data.Entities;
using LW.Services.TagServices;
using LW.Shared.DTOs.Tag;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }
         [HttpGet("GetAllTag")]
        public async Task<ActionResult<ApiResult<TagDto>>> GetAllTag()
        {
            var result = await _tagService.GetAllTag();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        
        
        [HttpGet("GetAllTagPagination")]
        public async Task<ActionResult<ApiResult<PagedList<TagDto>>>> GetAllTagPagination(
            [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _tagService.GetAllTagPagination(pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }
        
        [HttpGet("SearchByTagPagination")]
        public async Task<ActionResult<ApiResult<PagedList<TagDto>>>> SearchByTagPagination(
            [FromQuery] SearchTagDto searchTagDto)
        {
            var result = await _tagService.SearchTagPagination(searchTagDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetTagById")]
        public async Task<ActionResult<ApiResult<TagDto>>> GetTagById(int id)
        {
            var result = await _tagService.GetTagById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateTag")]
        public async Task<ActionResult<ApiResult<bool>>> CreateTag([FromBody] TagCreateDto model)
        {
            var validationResult = await new CreateTagCommandValidator().ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _tagService.CreateTag(model);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateTag")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateTag([FromBody] TagUpdateDto model)
        {
            var validationResult = await new UpdateTagCommandValidator().ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _tagService.UpdateTag(model);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateStatusTag/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusTag(int id)
        {
            var result = await _tagService.UpdateTagStatus(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteTag/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteTag(int id)
        {
            var result = await _tagService.DeleteTag(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

       
    }
}
