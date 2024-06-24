using LW.Shared.DTOs.File;
using Microsoft.AspNetCore.Http;

namespace LW.Contracts.Common;

public interface ICloudinaryService
{
    Task<FileImageDto> CreateImageAsync(IFormFile file, string folderName);
    Task<FileImageDto> UpdateImageAsync(string publicId, IFormFile file);
    Task<bool> DeleteImageAsync(string publicId);
    Task<FileDto> CreateFileAsync(IFormFile file, string folderName);
    Task<FileDto> UpdateFileAsync(string publicId, IFormFile file);
    Task<bool> DeleteFileAsync(string publicId);
    Task<bool> DeleteRangeFileAsync(IEnumerable<string> publicIds);
    Task<bool> DeleteFolderAsync(string folderName);
}