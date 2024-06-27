using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.CommentDocumentValidator;
using LW.Services.CommentDocumentServices;
using LW.Shared.DTOs.CommentDocumentDto;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentDocumentController : ControllerBase
    {
        private readonly ICommentDocumentService _commentDocumentService;

        public CommentDocumentController(ICommentDocumentService commentDocumentService)
        {
            _commentDocumentService = commentDocumentService;
        }

        [HttpGet("GetAllCommentByDocumentIdPagination")]
        public async Task<ActionResult<ApiResult<PagedList<PagedList<CommentDocumentDto>>>>>
            GetAllCommentByDocumentIdPagination([FromQuery] int documentId,
                [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result =
                await _commentDocumentService.GetAllCommentByDocumentIdPagination(documentId,
                    pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetAllCommentDocumentByUserIdPagination")]
        public async Task<ActionResult<ApiResult<PagedList<PagedList<CommentDocumentDto>>>>>
            GetAllCommentDocumentByUserIdPagination([FromQuery] string userId,
                [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result =
                await _commentDocumentService.GetAllCommentDocumentByUserIdPagination(userId, pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetCommentDocumentById/{commentDocumentId}")]
        public async Task<ActionResult<ApiResult<CommentDocumentDto>>> GetCommentDocumentById(
            [Required] int commentDocumentId)
        {
            var result = await _commentDocumentService.GetCommentDocumentById(commentDocumentId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateCommentDocument")]
        public async Task<ActionResult<ApiResult<CommentDocumentDto>>> CreateCommentDocument(
            CommentDocumentCreateDto commentDocumentCreateDto)
        {
            var validationResult =
                await new CreateCommentDocumentCommandValidator().ValidateAsync(commentDocumentCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _commentDocumentService.CreateCommentDocument(commentDocumentCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateCommentDocument")]
        public async Task<ActionResult<ApiResult<CommentDocumentDto>>> UpdateCommentDocument(
            CommentDocumentUpdateDto commentDocumentUpdateDto)
        {
            var validationResult =
                await new UpdateCommentDocumentCommandValidator().ValidateAsync(commentDocumentUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _commentDocumentService.UpdateCommentDocument(commentDocumentUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteCommentDocument/{commentDocumentId}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteCommentDocument([Required] int commentDocumentId)
        {
            var result = await _commentDocumentService.DeleteCommentDocument(commentDocumentId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}