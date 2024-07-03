using System.Net;
using Aspose.Pdf;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LW.Contracts.Common;
using LW.Shared.Configurations;
using LW.Shared.Constant;
using LW.Shared.DTOs.File;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using Serilog;
using ResourceType = CloudinaryDotNet.Actions.ResourceType;

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
        string decodedUrl = string.Empty;
        if (file == null || file.Length == 0)
        {
            _logger.Error("File cannot be null or empty.");
            return null;
        }

        if (string.IsNullOrEmpty(publicId))
        {
            var creteFile = await CreateImageAsync(file, CloudinaryConstant.FolderUserImage);
            var createFileImage = new FileImageDto()
            {
                PublicId = creteFile.PublicId,
                Url = creteFile.Url
            };

            return createFileImage;
        }

        decodedUrl = Uri.UnescapeDataString(publicId);
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
        string decodedUrl = string.Empty;
        if (file == null || file.Length == 0)
        {
            _logger.Error("File cannot be null or empty.");
            return null;
        }

        if (string.IsNullOrEmpty(publicId))
        {
            return null;
        }

        decodedUrl = Uri.UnescapeDataString(publicId);
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

    public async Task<bool> DeleteRangeFileAsync(IEnumerable<string> publicIds)
    {
        var decodedUrl = publicIds.Select(Uri.UnescapeDataString).ToArray();
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

    public async Task<FileDto> ConvertHtmlToPdf(string htmlContent, string fileName, string folderName)
    {
        if (string.IsNullOrEmpty(htmlContent) || string.IsNullOrEmpty(fileName))
        {
            _logger.Information("htmlContent or fileName is null or empty !!!");
            return null;
        }

        if (!fileName.Trim().EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".pdf";
        }

        await new BrowserFetcher().DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });
        await using var page = await browser.NewPageAsync();
        await page.SetContentAsync(htmlContent);
        var pdfStream = await page.PdfStreamAsync(new PdfOptions()
        {
            Format = PaperFormat.A4,
            DisplayHeaderFooter = true,
            MarginOptions = new MarginOptions
            {
                Top = "20px",
                Right = "20px",
                Bottom = "20px",
                Left = "20px"
            },
        });
        _logger.Information("Convert file HTML to PDF successfully !!!");
        if (pdfStream != null && pdfStream.Length > 0)
        {
            await using (var ms = new MemoryStream())
            {
                await pdfStream.CopyToAsync(ms);
                ms.Position = 0;

                IFormFile formFile = new FormFile(ms, 0, ms.Length, "pdfName", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/pdf"
                };
                var result = await CreateFileAsync(formFile, folderName);
                _logger.Information("Upload file successfully !!!");
                return result;
            }
        }

        return null;
    }

    public async Task<string> ConvertPdfToHtml(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            _logger.Error("File cannot be null or empty.");
            return null;
        }

        string htmlContent;

        var document = new Document(file.OpenReadStream());

        await using (var outputStream = new MemoryStream())
        {
            var saveOptions = new HtmlSaveOptions()
            {
                FixedLayout = true,
                SaveFullFont = true,
                UseZOrder = true,
                RasterImagesSavingMode = HtmlSaveOptions.RasterImagesSavingModes.AsEmbeddedPartsOfPngPageBackground,
                FontSavingMode = HtmlSaveOptions.FontSavingModes.SaveInAllFormats,
                PartsEmbeddingMode = HtmlSaveOptions.PartsEmbeddingModes.EmbedAllIntoHtml,
                LettersPositioningMethod = HtmlSaveOptions.LettersPositioningMethods
                    .UseEmUnitsAndCompensationOfRoundingErrorsInCss,
            };

            document.Save(outputStream, saveOptions);

            outputStream.Position = 0;

            using (var reader = new StreamReader(outputStream))
            {
                htmlContent = await reader.ReadToEndAsync();
            }
        }

        return htmlContent;
    }
}