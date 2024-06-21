using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LW.Contracts.Common;
using LW.Shared.Configurations;
using LW.Shared.Constant;
using LW.Shared.DTOs.File;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;

namespace LW.Infrastructure.Common;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger _logger;

    public CloudinaryService(IOptions<CloudinarySettings> cloudinarySettings, ILogger logger)
    {
        _logger = logger;
        var settings = cloudinarySettings.Value;
        var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<FileImageDto> CreateImageAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
        {
            _logger.Error("File cannot be null or empty.");
            return null;
        }

        var result = new ImageUploadResult();
        await using (var stream = file.OpenReadStream())
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folderName,
                PublicId = CloudinaryConstant.Image + Guid.NewGuid(),
                DisplayName = file.FileName,
                UseFilename = true
            };

            result = await _cloudinary.UploadAsync(uploadParams);
        }

        var fileImage = new FileImageDto()
        {
            PublicId = result.PublicId,
            Url = result.SecureUrl.ToString()
        };

        return fileImage;
    }

    public async Task<FileImageDto> UpdateImageAsync(string publicId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            _logger.Error("File cannot be null or empty.");
            return null;
        }

        string decodedUrl = Uri.UnescapeDataString(publicId);
        await _cloudinary.DestroyAsync(new DeletionParams(decodedUrl));

        var result = new ImageUploadResult();
        await using (var stream = file.OpenReadStream())
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = decodedUrl.Split('/')[0],
                PublicId = CloudinaryConstant.Image + Guid.NewGuid(),
                DisplayName = file.FileName,
                UseFilename = true
            };

            result = await _cloudinary.UploadAsync(uploadParams);
        }

        var fileImage = new FileImageDto()
        {
            PublicId = result.PublicId,
            Url = result.SecureUrl.ToString()
        };

        return fileImage;
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        string decodedUrl = Uri.UnescapeDataString(publicId);
        var result = await _cloudinary.DestroyAsync(new DeletionParams(decodedUrl));
        if (result.StatusCode != HttpStatusCode.OK)
        {
            _logger.Information(result.Error.Message);
            return false;
        }

        return true;
    }

    public async Task<FileDto> CreateFileAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
        {
            _logger.Error("File cannot be null or empty.");
            return null;
        }

        var result = new RawUploadResult();
        await using (var stream = file.OpenReadStream())
        {
            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folderName,
                PublicId = CloudinaryConstant.File + Guid.NewGuid(),
                DisplayName = file.FileName,
                UseFilename = true
            };

            result = await _cloudinary.UploadAsync(uploadParams);
        }

        var fileDownload = new FileDto()
        {
            PublicId = result.PublicId,
            Url = result.SecureUrl.ToString(),
            UrlDownload = _cloudinary.Api.Url.ResourceType("raw").Action("upload")
                .Transform(new Transformation().Flags("attachment")).BuildUrl(result.PublicId)
        };

        return fileDownload;
    }

    public async Task<FileDto> UpdateFileAsync(string publicId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            _logger.Error("File cannot be null or empty.");
            return null;
        }

        string decodedUrl = Uri.UnescapeDataString(publicId);
        await _cloudinary.DeleteResourcesAsync(ResourceType.Raw, decodedUrl);

        var result = new RawUploadResult();
        await using (var stream = file.OpenReadStream())
        {
            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = decodedUrl.Split('/')[0],
                PublicId = CloudinaryConstant.File + Guid.NewGuid(),
                DisplayName = file.FileName,
                UseFilename = true
            };

            result = await _cloudinary.UploadAsync(uploadParams);
        }

        var fileDownload = new FileDto()
        {
            PublicId = result.PublicId,
            Url = result.SecureUrl.ToString(),
            UrlDownload = _cloudinary.Api.Url.ResourceType("raw").Action("upload")
                .Transform(new Transformation().Flags("attachment")).BuildUrl(result.PublicId)
        };

        return fileDownload;
    }

    public async Task<bool> DeleteFileAsync(string publicId)
    {
        string decodedUrl = Uri.UnescapeDataString(publicId);
        var result = await _cloudinary.DeleteResourcesAsync(ResourceType.Raw, decodedUrl);
        if (result.StatusCode != HttpStatusCode.OK)
        {
            _logger.Error(result.Error.Message);
            return false;
        }

        return true;
    }

    public async Task<bool> DeleteFolderAsync(string folderName)
    {
        string decodedUrl = Uri.UnescapeDataString(folderName);
        var result = await _cloudinary.DeleteFolderAsync(decodedUrl);
        if (result.StatusCode != HttpStatusCode.OK)
        {
            _logger.Error(result.Error.Message);
            return false;
        }

        return true;
    }
}