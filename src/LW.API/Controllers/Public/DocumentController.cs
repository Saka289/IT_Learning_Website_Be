using LW.Services.DocumentService;
using LW.Shared.DTOs.Document;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using LW.API.Application.Validators.DocumentValidator;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("GetAllDocument")]
        public async Task<ActionResult<ApiResult<IEnumerable<DocumentDto>>>> GetAllDocument()
        {
            var result = await _documentService.GetAllDocument();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetAllDocumentPagination")]
        public async Task<ActionResult<ApiResult<PagedList<DocumentDto>>>> GetAllDocumentPagination([FromQuery]PagingRequestParameters pagingRequestParameters)
        {
            var result = await _documentService.GetAllDocumentPagination(pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetDocumentById/{id}")]
        public async Task<ActionResult<ApiResult<DocumentDto>>> GetDocumentById([Required] int id)
        {
            var result = await _documentService.GetDocumentById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("SearchByDocumentPagination")]
        public async Task<ActionResult<ApiResult<PagedList<DocumentDto>>>> SearchByDocumentPagination([FromQuery] SearchDocumentDto searchDocumentDto)
        {
            var result = await _documentService.SearchByDocumentPagination(searchDocumentDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpPost("CreateDocument")]
        public async Task<ActionResult<ApiResult<DocumentDto>>> CreateDocument([FromBody] DocumentCreateDto documentCreateDto)
        {
            var validationResult = await new CreateDocumentCommandValidator().ValidateAsync(documentCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            var result = await _documentService.CreateDocument(documentCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }

        [HttpPut("UpdateDocument")]
        public async Task<ActionResult<ApiResult<DocumentDto>>> UpdateDocument([FromBody] DocumentUpdateDto documentUpdateDto)
        {
            var validationResult = await new UpdateDocumentCommandValidator().ValidateAsync(documentUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            var result = await _documentService.UpdateDocument(documentUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        
        
        [HttpPut("UpdateStatusDocument")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusDocument(int id)
        {
            var result = await _documentService.UpdateStatusDocument(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            
            return Ok(result);
        }

        [HttpDelete("DeleteDocument/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteDocument([Required] int id)
        {
            var result = await _documentService.DeleteDocument(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            
            return Ok(result);
        }
    }

}
