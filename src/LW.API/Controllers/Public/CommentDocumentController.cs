using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet("GetAllCommentByDocumentIdPagination/{documentId}")]
        public async Task<ActionResult<ApiResult<PagedList<PagedList<CommentDocumentDto>>>>>
            GetAllCommentByDocumentIdPagination([FromQuery] int documentId,
                [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _commentDocumentService.GetAllCommentByDocumentIdPagination(documentId, pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetAllCommentDocumentByUserIdPagination/{userId}")]
        public async Task<ActionResult<ApiResult<PagedList<PagedList<CommentDocumentDto>>>>>
            GetAllCommentDocumentByUserIdPagination([FromQuery] string userId,
                [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _commentDocumentService.GetAllCommentDocumentByUserIdPagination(userId, pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetCommentDocumentById/{documentId}")]
        public async Task<ActionResult<ApiResult<CommentDocumentDto>>> GetCommentDocumentById([Required] int documentId)
        {
            var result = await _commentDocumentService.GetCommentDocumentById(documentId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        // public async Task<ActionResult<ApiResult<CommentDocumentDto>>> CreateCommentDocument(
        //     CommentDocumentCreateDto commentDocumentCreateDto)
        // {
        //     return Ok();
        // }
    }
}