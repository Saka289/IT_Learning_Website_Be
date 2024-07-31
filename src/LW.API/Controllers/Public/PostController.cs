using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.PostValidator;
using LW.Services.PostServices;
using LW.Shared.DTOs.Post;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
            
        }
         [HttpGet("GetAllPost")]
        public async Task<ActionResult<ApiResult<PostDto>>> GetAllPost()
        {
            var result = await _postService.GetAllPost();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("GetAllPostByGrade/{gradeId}")]
        public async Task<ActionResult<ApiResult<IEnumerable<PostDto>>>> GetAllPostByGrade(int gradeId)
        {
            var result = await _postService.GetAllPostByGrade(gradeId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("GetAllPostByUser/{userId}")]
        public async Task<ActionResult<ApiResult<IEnumerable<PostDto>>>> GetAllPostByUser(string userId)
        {
            var result = await _postService.GetAllPostByUser(userId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        
        
        [HttpGet("GetAllPostPagination")]
        public async Task<ActionResult<ApiResult<PagedList<PostDto>>>> GetAllPostPagination(
            [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _postService.GetAllPostPagination(pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }
        [HttpGet("GetAllPostByGradePagination")]
        public async Task<ActionResult<ApiResult<PagedList<PostDto>>>> GetAllPostByGradePagination([Required] int gradeId,
            [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _postService.GetAllPostByGradePagination(gradeId, pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }
        [HttpGet("GetAllPostByUserPagination")]
        public async Task<ActionResult<ApiResult<PagedList<PostDto>>>> GetAllPostByUserPagination([Required] string userId,
            [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _postService.GetAllPostByUserPagination(userId, pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }
    

        [HttpGet("GetPostById")]
        public async Task<ActionResult<ApiResult<PostDto>>> GetPostById(int id)
        {
            var result = await _postService.GetPostById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("CreatePost")]
        public async Task<ActionResult<ApiResult<bool>>> CreatePost([FromBody] PostCreateDto model)
        {
            var validationResult = await new CreatePostCommandValidator().ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _postService.CreatePost(model);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdatePost")]
        public async Task<ActionResult<ApiResult<bool>>> UpdatePost([FromBody] PostUpdateDto model)
        {
            var validationResult = await new UpdatePostCommandValidator().ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _postService.UpdatePost(model);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

       

        [HttpDelete("DeletePost/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeletePost(int id)
        {
            var result = await _postService.DeletePost(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}