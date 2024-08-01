using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.PostCommentValidator;
using LW.Services.PostCommentServices;
using LW.Shared.DTOs.PostComment;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostCommentController : ControllerBase
    {
        private readonly IPostCommentService _postCommentService;

        public PostCommentController(IPostCommentService postCommentService)
        {
            _postCommentService = postCommentService;
        }

        [HttpGet("GetAllCommentByPostIdPagination")]
        public async Task<ActionResult<ApiResult<PagedList<PostCommentDto>>>> GetAllCommentByPostIdPagination(
            [Required] int postId, [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _postCommentService.GetAllPostCommentByPostIdPagination(postId, pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetAllCommentByParentIdPagination")]
        public async Task<ActionResult<ApiResult<PagedList<PostCommentDto>>>> GetAllCommentByParentIdPagination(
            [Required] int parentId, [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result =
                await _postCommentService.GetAllPostCommentByParentIdPagination(parentId, pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }


        [HttpGet("GetPostCommentById/{id}")]
        public async Task<ActionResult<ApiResult<PostCommentDto>>> GetCommentDocumentById(
            int id)
        {
            var result = await _postCommentService.GetPostCommentById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("CreatePostComment")]
        public async Task<ActionResult<ApiResult<PostCommentDto>>> CreatePostComment(
            PostCommentCreateDto postCommentCreateDto)
        {
            var validationResult =
                await new CreatePostCommentCommandValidator().ValidateAsync(postCommentCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _postCommentService.CreatePostComment(postCommentCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdatePostComment")]
        public async Task<ActionResult<ApiResult<PostCommentDto>>> UpdatePostComment(
            PostCommentUpdateDto postCommentUpdateDto)
        {
            var validationResult =
                await new UpdatePostCommentCommandValidator().ValidateAsync(postCommentUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _postCommentService.UpdatePostComment(postCommentUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeletePostComment/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeletePostComment(int id)
        {
            var result = await _postCommentService.DeletePostComment(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("VotePostComment")]
        public async Task<ActionResult<ApiResult<bool>>> VotePostComment(
           [Required] int commentId,[Required] string userId)
        {
            var result = await _postCommentService.VoteCorrectPostComment(commentId, userId);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}