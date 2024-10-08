using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.Contracts.Common;
using LW.Shared.Constant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleConstant.RoleAdmin)]
    public class FileController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;

        public FileController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("CreateImage")]
        public async Task<IActionResult> CreateImage(IFormFile file, [Required] string folderName)
        {
            var result = await _cloudinaryService.CreateImageAsync(file, folderName);
            return Ok(result);
        }

        [HttpPut("UpdateImage/{publicId}")]
        public async Task<IActionResult> UpdateImage([Required] string publicId, IFormFile file)
        {
            var result = await _cloudinaryService.UpdateImageAsync(publicId, file);
            return Ok(result);
        }

        [HttpDelete("DeleteImage/{publicId}")]
        public async Task<IActionResult> DeleteImage([Required] string publicId)
        {
            var result = await _cloudinaryService.DeleteImageAsync(publicId);
            return Ok(result);
        }
       
        [HttpPost("CreateFile")]
        public async Task<IActionResult> CreateFile(IFormFile file, [Required] string folderName)
        {
            var result = await _cloudinaryService.CreateFileAsync(file, folderName);
            return Ok(result);
        }

        [HttpPut("UpdateFile/{publicId}")]
        public async Task<IActionResult> UpdateFile([Required] string publicId, IFormFile file)
        {
            var result = await _cloudinaryService.UpdateFileAsync(publicId, file);
            return Ok(result);
        }

        [HttpDelete("DeleteFile/{publicId}")]
        public async Task<IActionResult> DeleteFile([Required] string publicId)
        {
            var result = await _cloudinaryService.DeleteFileAsync(publicId);
            return Ok(result);
        }

        [HttpDelete("DeleteFolder/{folderName}")]
        public async Task<IActionResult> DeleteFolder([Required] string folderName)
        {
            var result = await _cloudinaryService.DeleteFolderAsync(folderName);
            return Ok(result);
        }

        [HttpPost("ConvertFileHtmlToPdf")]
        public async Task<IActionResult> ConvertFileHtmlToPdf([FromBody] string htmlContent, string fileName, string folderName)
        {
            var result = await _cloudinaryService.ConvertHtmlToPdf(htmlContent, fileName, folderName);
            return Ok(result);
        }

        [HttpPost("ConvertFilePdfToHtml")]
        public async Task<IActionResult> ConvertFilePdfToHtml(IFormFile file)
        {
            var result = await _cloudinaryService.ConvertPdfToHtml(file);
            return Ok(result);
        }
    }
}