using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.ExamImageRepositories;
using LW.Data.Repositories.ExamRepositories;
using LW.Shared.Constant;
using LW.Shared.DTOs;
using LW.Shared.DTOs.File;
using LW.Shared.SeedWork;

namespace LW.Services.ExamImageServices;

public class ExamImageService : IExamImageService
{
    private readonly IExamImageRepository _examImageRepository;
    private readonly IMapper _mapper;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IExamRepository _examRepository;

    public ExamImageService(IExamImageRepository examImageRepository, IMapper mapper,
        ICloudinaryService cloudinaryService, IExamRepository examRepository)
    {
        _examImageRepository = examImageRepository;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
        _examRepository = examRepository;
    }

    public async Task<ApiResult<IEnumerable<ExamImageDto>>> GetAllExamImage()
    {
        var listExamImage = await _examImageRepository.GetAllExamImage();
        if (listExamImage.Count() == 0)
        {
            return new ApiResult<IEnumerable<ExamImageDto>>(false, "Not found");
        }

        var result = _mapper.Map<IEnumerable<ExamImageDto>>(listExamImage);
        return new ApiResult<IEnumerable<ExamImageDto>>(true, result, "Get all exam image");
    }

    public async Task<ApiResult<ExamImageDto>> GetExamImageById(int id)
    {
        var examImage = await _examImageRepository.GetExamImageById(id);
        if (examImage == null)
        {
            return new ApiResult<ExamImageDto>(false, "Not found");
        }

        var result = _mapper.Map<ExamImageDto>(examImage);
        return new ApiResult<ExamImageDto>(true, result, "Get exam image by id successfully");
    }

    public async Task<ApiResult<IEnumerable<ExamImageDto>>> GetExamImageByExamId(int examId)
    {
        var list = await _examImageRepository.GetAllExamImageByExamId(examId);
        if (list.Count() == 0)
        {
            return new ApiResult<IEnumerable<ExamImageDto>>(false, "Not found");
        }

        var result = _mapper.Map<IEnumerable<ExamImageDto>>(list);
        return new ApiResult<IEnumerable<ExamImageDto>>(true, result, "Get all image by exam id");
    }

    public async Task<ApiResult<ExamImageDto>> CreateExamImage(ExamImageCreateDto examImageCreateDto)
    {
        var imageDto = new FileImageDto();
        var exam = await _examRepository.GetExamById(examImageCreateDto.ExamId);
        if (exam == null)
        {
            return new ApiResult<ExamImageDto>(false, "Not found exam");
        }

        var listImage = await _examImageRepository.GetAllExamImageByExamId(exam.Id);
        int stt = listImage.Count() > 0 ? listImage.Count() + 1 : 1;

        imageDto = await _cloudinaryService.CreateImageAsync(examImageCreateDto.ImageUpload,
            CloudinaryConstant.FolderExamImage);
        var examImage = _mapper.Map<ExamImage>(examImageCreateDto);
        examImage.publicId = imageDto.PublicId;
        examImage.FilePath = imageDto.Url;
        examImage.Index = stt;
        await _examImageRepository.CreateExamImage(examImage);
        var result = _mapper.Map<ExamImageDto>(examImage);
        return new ApiResult<ExamImageDto>(true, result, "Create ExamImage For This Exam Successfully");
    }

    public async Task<ApiResult<ExamImageDto>> UpdateExamImage(ExamImageUpdateDto examImageUpdateDto)
    {
        var image = await _examImageRepository.GetExamImageById(examImageUpdateDto.Id);
        if (image == null)
        {
            return new ApiResult<ExamImageDto>(false, "Not found examimage");
        }
        var exam = await _examRepository.GetExamById(examImageUpdateDto.ExamId);
        if (exam == null)
        {
            return new ApiResult<ExamImageDto>(false, "Not found exam");
        }

        var examImageEntity = _mapper.Map<ExamImage>(examImageUpdateDto);
        if (examImageUpdateDto.ImageUpload != null && examImageUpdateDto.ImageUpload.Length > 0)
        {
            FileImageDto filePath =
                await _cloudinaryService.UpdateImageAsync(image.publicId, examImageUpdateDto.ImageUpload);
            examImageEntity.publicId = filePath.PublicId;
            examImageEntity.FilePath = filePath.Url;
        }
        await _examImageRepository.UpdateExamImage(examImageEntity);
        var result = _mapper.Map<ExamImageDto>(examImageEntity);
        return new ApiResult<ExamImageDto>(true,result, "Update image of exam successfully");
    }

    public async Task<ApiResult<bool>> DeleteExamImage(int id)
    {
        var examImage = await _examImageRepository.GetExamImageById(id);
        if (examImage == null)
        {
            return new ApiResult<bool>(false, "Not Found");
        }

        await _examImageRepository.DeleteExamImage(id);
        await _cloudinaryService.DeleteImageAsync(examImage.publicId);
        return new ApiResult<bool>(true, "Delete image of exam successfully");
    }
}