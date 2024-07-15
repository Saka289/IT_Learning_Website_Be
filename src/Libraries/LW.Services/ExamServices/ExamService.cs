using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.ExamImageRepositories;
using LW.Data.Repositories.ExamRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs;
using LW.Shared.DTOs.Exam;
using LW.Shared.DTOs.File;
using LW.Shared.DTOs.Lesson;
using LW.Shared.SeedWork;
using MockQueryable.Moq;

namespace LW.Services.ExamServices;

public class ExamService : IExamService
{
    private readonly IMapper _mapper;
    private readonly IExamRepository _examRepository;
    private readonly IElasticSearchService<ExamDto, int> _elasticSearchService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IExamImageRepository _examImageRepository;

    public ExamService(IMapper mapper, IExamRepository examRepository,
        IElasticSearchService<ExamDto, int> elasticSearchService, ICloudinaryService cloudinaryService, IExamImageRepository examImageRepository)
    {
        _mapper = mapper;
        _examRepository = examRepository;
        _elasticSearchService = elasticSearchService;
        _cloudinaryService = cloudinaryService;
        _examImageRepository = examImageRepository;
    }

    public async Task<ApiResult<IEnumerable<ExamDto>>> GetAllExam()
    {
        var listExam = await _examRepository.GetAllExam();
        if (listExam.Count() == 0)
        {
            return new ApiResult<IEnumerable<ExamDto>>(false, "Not Found");
        }

        var result = _mapper.Map<IEnumerable<ExamDto>>(listExam);
        return new ApiResult<IEnumerable<ExamDto>>(true, result, "Get list exam successfully");
    }

    public async Task<ApiResult<PagedList<ExamDto>>> GetAllExamPagination(
        PagingRequestParameters pagingRequestParameters)
    {
        var examList = await _examRepository.GetAllExamByPagination();
        if (examList == null)
        {
            return new ApiResult<PagedList<ExamDto>>(false, "Exam is null !!!");
        }

        var result = _mapper.ProjectTo<ExamDto>(examList);
        var pagedResult = await PagedList<ExamDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);
        return new ApiSuccessResult<PagedList<ExamDto>>(pagedResult);
    }

    public async Task<ApiResult<ExamDto>> GetExamById(int id)
    {
        var exam = await _examRepository.GetExamById(id);
        if (exam == null)
        {
            return new ApiResult<ExamDto>(false, "Not Found");
        }

        var result = _mapper.Map<ExamDto>(exam);
        
        // var listExamImage = await _examImageRepository.GetAllExamImageByExamId(id);
        // if (listExamImage != null && listExamImage.Count() > 0)
        // {
        //     var listExamImageDto = _mapper.Map<IEnumerable<ExamImageDto>>(listExamImage);
        //     result.ImageDtos = listExamImageDto;
        // }

        return new ApiResult<ExamDto>(true, result, "Get Exam By Id Successfully");
    }

    public async Task<ApiResult<PagedList<ExamDto>>> SearchByExamPagination(SearchExamDto searchExamDto)
    {
        var listExam =
            await _elasticSearchService.SearchDocumentAsync(ElasticConstant.ElasticExams, searchExamDto);
        if (listExam is null)
        {
            return new ApiResult<PagedList<ExamDto>>(false, $"Lesson not found by {searchExamDto.Key} !!!");
        }

        var result = _mapper.Map<IEnumerable<ExamDto>>(listExam);
        var pagedResult = await PagedList<ExamDto>.ToPageListAsync(result.AsQueryable().BuildMock(),
            searchExamDto.PageIndex, searchExamDto.PageSize, searchExamDto.OrderBy, searchExamDto.IsAscending);
        return new ApiSuccessResult<PagedList<ExamDto>>(pagedResult);
    }

    public async Task<ApiResult<ExamDto>> CreateExam(ExamCreateDto examCreateDto)
    {
        var obj = _mapper.Map<Exam>(examCreateDto);
        var filePath = new FileDto();
        if (examCreateDto.ExamFile != null && examCreateDto.ExamFile.Length > 0)
        {
            filePath = await _cloudinaryService.CreateFileAsync(examCreateDto.ExamFile,
                CloudinaryConstant.FolderExamFilePdf);
            obj.ExamFile = filePath.Url;
            obj.PublicId = filePath.PublicId;
        }
        var keyWordValue = "";
        if (examCreateDto.tagValues.Any())
        {
            keyWordValue = string.Join(",", examCreateDto.tagValues);
        }

        obj.KeyWord = keyWordValue;
        await _examRepository.CreateExam(obj);
        var result = _mapper.Map<ExamDto>(obj);
        await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticExams, result, x => x.Id);
        // nếu upload đề theo ảnh
        if (examCreateDto.Images != null && examCreateDto.Images.Count() > 0)
        {
            var examImages = new List<ExamImage>();
            var stt = 1;
            foreach (var image in examCreateDto.Images)
            {
                var fileImageDto = new FileImageDto();
                fileImageDto = await _cloudinaryService.CreateImageAsync(image, CloudinaryConstant.FolderExamImage);
                var examImage = new ExamImage()
                {
                    FilePath = fileImageDto.Url,
                    publicId = fileImageDto.PublicId,
                    ExamId = result.Id,
                    Index = stt++
                };
                examImages.Add(examImage);
            }
            await _examImageRepository.CreateRangeExamImage(examImages);
        }
        return new ApiResult<ExamDto>(true, result, "Create exam successfully");
    }

    public async Task<ApiResult<ExamDto>> UpdateExam(ExamUpdateDto examUpdateDto)
    {
        var exam = await _examRepository.GetExamById(examUpdateDto.Id);
        if (exam == null)
        {
            return new ApiResult<ExamDto>(false, "Not Found");
        }

        var objUpdate = _mapper.Map(examUpdateDto, exam);
        if (examUpdateDto.FileUpload != null && examUpdateDto.FileUpload.Length > 0)
        {
            var filePath = new FileDto();
            filePath = await _cloudinaryService.UpdateFileAsync(exam.PublicId, examUpdateDto.FileUpload);
            objUpdate.PublicId = filePath.PublicId;
            objUpdate.ExamFile = filePath.Url;
        }
        var keyWordValue = "";
        if (examUpdateDto.tagValues!= null &&  examUpdateDto.tagValues.Any())
        {
            keyWordValue = string.Join(",", examUpdateDto.tagValues);
            objUpdate.KeyWord = keyWordValue;
        }
        await _examRepository.UpdateExam(objUpdate);
        var examDto = _mapper.Map<ExamDto>(objUpdate);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticExams, examDto, examUpdateDto.Id);
        return new ApiResult<ExamDto>(true, examDto, "Update exam successfully");
    }

    public async Task<ApiResult<bool>> UpdateExamStatus(int id)
    {
        var exam = await _examRepository.GetExamById(id);
        if (exam == null)
        {
            return new ApiResult<bool>(false, "Not found");
        }

        exam.IsActive = !exam.IsActive;
        await _examRepository.UpdateExam(exam);
        var examDto = _mapper.Map<ExamDto>(exam);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticExams, examDto, exam.Id);
        return new ApiResult<bool>(true, "Update status exam successfully");
    }

    public async Task<ApiResult<bool>> DeleteExam(int id)
    {
        var exam = await _examRepository.GetExamById(id);
        if (exam == null)
        {
            return new ApiResult<bool>(false, "Not Found");
        }
        // delete all images exam if have
        var listImage = await _examImageRepository.GetAllExamImageByExamId(id);
        if (listImage.Count() > 0)
        {
            foreach (var image in listImage)
            {
               await _cloudinaryService.DeleteImageAsync(image.publicId);
            }
        }
        // delete file pdf 
        if (!string.IsNullOrEmpty(exam.ExamFile))
        {
            await _cloudinaryService.DeleteFileAsync(exam.PublicId);
        }
        var result = await _examRepository.DeleteExam(id);
        if (result == false)
        {
            return new ApiResult<bool>(false, "Delete Exam Failed");
        }

        await _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticExams, exam.Id);
        return new ApiResult<bool>(true, "Delete Exam Successfully");
    }
}