using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.ExamCodeRepositories;
using LW.Data.Repositories.ExamRepositories;
using LW.Shared.Constant;
using LW.Shared.DTOs.ExamCode;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;

namespace LW.Services.ExamCodeServices;

public class ExamCodeService : IExamCodeService
{
    private readonly IExamCodeRepository _examCodeRepository;
    private readonly IMapper _mapper;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IElasticSearchService<ExamCodeDto, int> _elasticSearchService;
    private readonly IExamRepository _examRepository;

    public ExamCodeService(IExamCodeRepository examCodeRepository, IMapper mapper, ICloudinaryService cloudinaryService,
        IElasticSearchService<ExamCodeDto, int> elasticSearchService, IExamRepository examRepository)
    {
        _examCodeRepository = examCodeRepository;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
        _elasticSearchService = elasticSearchService;
        _examRepository = examRepository;
    }

    public async Task<ApiResult<IEnumerable<ExamCodeDto>>> GetAllExamCode()
    {
        var codes = await _examCodeRepository.GetAllExamCode();
        if (codes.Count() == 0)
        {
            return new ApiResult<IEnumerable<ExamCodeDto>>(false, "Not Found");
        }

        var result = _mapper.Map<IEnumerable<ExamCodeDto>>(codes);
        return new ApiResult<IEnumerable<ExamCodeDto>>(true, result, "Get all exam code successfully");
    }

    public async Task<ApiResult<ExamCodeDto>> GetExamCodeById(int id)
    {
        var code = await _examCodeRepository.GetExamCodeById(id);
        if (code == null)
        {
            return new ApiResult<ExamCodeDto>(false, "Not Found");
        }

        var result = _mapper.Map<ExamCodeDto>(code);
        return new ApiResult<ExamCodeDto>(true, result, "Get ExamCode By Id Successfully");
    }

    public async Task<ApiResult<IEnumerable<ExamCodeDto>>> GetExamCodeByExamId(int examId)
    {
        var codes = await _examCodeRepository.GetAllExamCodeByExamId(examId);
        if (codes.Count() == 0)
        {
            return new ApiResult<IEnumerable<ExamCodeDto>>(false, "Not Found");
        }

        var result = _mapper.Map<IEnumerable<ExamCodeDto>>(codes);
        return new ApiResult<IEnumerable<ExamCodeDto>>(true, result, "Get all exam code by examId successfully");
    }

    public async Task<ApiResult<ExamCodeDto>> CreateExamCode(ExamCodeCreateDto examCodeCreateDto)
    {
        var exam = await _examRepository.GetExamById(examCodeCreateDto.ExamId);
        if (exam == null)
        {
            return new ApiResult<ExamCodeDto>(false, $"Not find exam with examId = {examCodeCreateDto.ExamId}");
        }
        // check code  is existed
        var codeExist =
            await _examCodeRepository.GetExamCodeByCode(examCodeCreateDto.Code);
        if (codeExist != null)
        {
            return new ApiResult<ExamCodeDto>(false,
                $"An code already exists with codeName = {examCodeCreateDto.Code}");
        }
        // check file
        if (examCodeCreateDto.ExamFileUpload == null)
        {
            return new ApiResult<ExamCodeDto>(false,
                $"Must upload file exam with this code");
        }

        var filePath =
            await _cloudinaryService.CreateFileAsync(examCodeCreateDto.ExamFileUpload,
                CloudinaryConstant.FolderExamFilePdf);
        
        var examCode = _mapper.Map<ExamCode>(examCodeCreateDto);
        examCode.ExamFile = filePath.Url;
        examCode.PublicExamId = filePath.PublicId;
        await _examCodeRepository.CreateExamCode(examCode);
        var result = _mapper.Map<ExamCodeDto>(examCode);
        return new ApiResult<ExamCodeDto>(true, result, "Create code for exam successfully");
    }

    public async Task<ApiResult<IEnumerable<ExamCodeDto>>> CreateRangeExamCode(ExamCodeCreateRangeDto ExamCodeCreateRangeDtos)
    {
        var exam = await _examRepository.GetExamById(ExamCodeCreateRangeDtos.ExamId);
        if (exam == null)
        {
            return new ApiResult<IEnumerable<ExamCodeDto>>(false, $"Not find exam with examId = {ExamCodeCreateRangeDtos.ExamId}");
        }
        foreach (var examCode  in ExamCodeCreateRangeDtos.CodeDtos)
        {
            var codeExist = await _examCodeRepository.GetExamCodeByCode(examCode.Code);
            if (codeExist != null)
            {
                return new ApiResult<IEnumerable<ExamCodeDto>>(false,
                    $"An code already exists with codeName = {examCode.Code}");
            }
        }

        var listExamCode = new List<ExamCode>();
        foreach (var codeDto in ExamCodeCreateRangeDtos.CodeDtos)
        {
            var filePath =
                await _cloudinaryService.CreateFileAsync(codeDto.ExamFileUpload, CloudinaryConstant.FolderExamFilePdf);
            listExamCode.Add(new ExamCode()
            {
                ExamId = ExamCodeCreateRangeDtos.ExamId,
                ExamFile = filePath.Url,
                PublicExamId = filePath.PublicId,
                Code = codeDto.Code,
            });
        }
        var rs = await _examCodeRepository.CreateRangeExamCode(listExamCode);
        var result = _mapper.Map<IEnumerable<ExamCodeDto>>(rs);
        return new ApiResult<IEnumerable<ExamCodeDto>>(true, result, "Create range code for exam successfully");
    }

    public async Task<ApiResult<ExamCodeDto>> UpdateExamCode(ExamCodeUpdateDto examCodeUpdateDto)
    {
        var examCode = await _examCodeRepository.GetExamCodeById(examCodeUpdateDto.Id);
        if (examCode == null)
        {
            return new ApiResult<ExamCodeDto>(false, "Not found exam code to update");
        }
        var exam = await _examRepository.GetExamById(examCodeUpdateDto.ExamId);
        if (exam == null)
        {
            return new ApiResult<ExamCodeDto>(false, $"Not find exam with examId = {examCodeUpdateDto.ExamId}");
        }
       
        var objUpdate = _mapper.Map(examCodeUpdateDto, examCode);
        if (examCodeUpdateDto.ExamFileUpload != null && examCodeUpdateDto.ExamFileUpload.Length > 0)
        {
            var filePath =
                await _cloudinaryService.UpdateFileAsync(examCode.PublicExamId, examCodeUpdateDto.ExamFileUpload);
            objUpdate.ExamFile = filePath.Url;
            objUpdate.PublicExamId = filePath.PublicId;
        }
        var code =  await _examCodeRepository.UpdateExamCode(objUpdate);
        var result = _mapper.Map<ExamCodeDto>(code);
        return new ApiResult<ExamCodeDto>(true, result,"Update exam code successfully");
    }

    public async Task<ApiResult<bool>> DeleteExamCode(int id)
    {
        var examCode = await _examCodeRepository.GetExamCodeById(id);
        if (examCode == null)
        {
            return new ApiResult<bool>(false, "Not found");
        }
        await _examCodeRepository.DeleteAsync(examCode);
        await _cloudinaryService.DeleteFileAsync(examCode.PublicExamId);
        return new ApiResult<bool>(true, "Delete exam code successfully");
    }
}