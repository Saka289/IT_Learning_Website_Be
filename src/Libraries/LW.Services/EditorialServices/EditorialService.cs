using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.EditorialRepositories;
using LW.Data.Repositories.ProblemRepositories;
using LW.Services.ProblemServices;
using LW.Shared.Constant;
using LW.Shared.DTOs.Editorial;
using LW.Shared.DTOs.File;
using LW.Shared.SeedWork;

namespace LW.Services.EditorialServices;

public class EditorialService : IEditorialService
{
    private readonly IEditorialRepository _editorialRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IProblemRepository _problemRepository;
    private readonly IMapper _mapper;

    public EditorialService(IEditorialRepository editorialRepository, IMapper mapper, ICloudinaryService cloudinaryService, IProblemRepository problemRepository)
    {
        _editorialRepository = editorialRepository;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
        _problemRepository = problemRepository;
    }
    
    public async Task<ApiResult<IEnumerable<EditorialDto>>> GetAllEditorial()
    {
        var editorialList = await _editorialRepository.GetAllEditorial();
        if (!editorialList.Any())
        {
            return new ApiResult<IEnumerable<EditorialDto>>(false, "Editorial not found !!!");
        }

        var result = _mapper.Map<IEnumerable<EditorialDto>>(editorialList);
        return new ApiSuccessResult<IEnumerable<EditorialDto>>(result);
    }

    public async Task<ApiResult<EditorialDto>> GetEditorialByProblemId(int problemId)
    {
        var editorial = await _editorialRepository.GetAllEditorialByProblemId(problemId);
        if (editorial is null)
        {
            return new ApiResult<EditorialDto>(false, "Editorial not found !!!");
        }

        var result = _mapper.Map<EditorialDto>(editorial);
        return new ApiSuccessResult<EditorialDto>(result);
    }

    public async Task<ApiResult<EditorialDto>> GetEditorialById(int id)
    {
        var editorial = await _editorialRepository.GetEditorialById(id);
        if (editorial is null)
        {
            return new ApiResult<EditorialDto>(false, "Editorial not found !!!");
        }

        var result = _mapper.Map<EditorialDto>(editorial);
        return new ApiSuccessResult<EditorialDto>(result);
    }

    public async Task<ApiResult<EditorialDto>> CreateEditorial(EditorialCreateDto editorialCreateDto)
    {
        var problem = await _problemRepository.GetProblemById(editorialCreateDto.ProblemId);
        if (problem is null)
        {
            return new ApiResult<EditorialDto>(false, "Problem not found !!!");
        }

        var imagePath = new FileImageDto();
        if (editorialCreateDto.Image != null && editorialCreateDto.Image.Length > 0)
        {
            imagePath = await _cloudinaryService.CreateImageAsync( editorialCreateDto.Image, CloudinaryConstant.FolderEditorialImage);
        }
        var editorialMapper = _mapper.Map<Editorial>(editorialCreateDto);
        editorialMapper.Image = imagePath.Url;
        editorialMapper.PublicId = imagePath.PublicId;
        var editorialCreate = await _editorialRepository.CreateEditorial(editorialMapper);
        var result = _mapper.Map<EditorialDto>(editorialCreate);
        return new ApiSuccessResult<EditorialDto>(result);
    }

    public async Task<ApiResult<EditorialDto>> UpdateEditorial(EditorialUpdateDto editorialUpdateDto)
    {
        var problem = await _problemRepository.GetProblemById(editorialUpdateDto.ProblemId);
        if (problem is null)
        {
            return new ApiResult<EditorialDto>(false, "Problem not found !!!");
        }

        var editorialEntity = await _editorialRepository.GetEditorialById(editorialUpdateDto.Id);
        if (editorialEntity is null)
        {
            return new ApiResult<EditorialDto>(false, "Editorial not found !!!");
        }

        var imagePath = new FileImageDto();
        if (editorialUpdateDto.Image != null && editorialUpdateDto.Image.Length > 0)
        {
            imagePath = await _cloudinaryService.UpdateImageAsync(CloudinaryConstant.FolderEditorialImage, editorialUpdateDto.Image);
        }
        else
        {
            imagePath.Url = editorialEntity.Image;
            imagePath.PublicId = editorialEntity.PublicId;
        }

        var editorial = _mapper.Map(editorialUpdateDto, editorialEntity);
        editorial.PublicId = imagePath.PublicId;
        editorial.Image = imagePath.Url;
        var editorialUpdate = await _editorialRepository.UpdateEditorial(editorial);
        var result = _mapper.Map<EditorialDto>(editorialUpdate);
        return new ApiSuccessResult<EditorialDto>(result);
    }

    public async Task<ApiResult<bool>> DeleteEditorial(int id)
    {
        var editorial = await _editorialRepository.GetEditorialById(id);
        if (editorial is null)
        {
            return new ApiResult<bool>(false, "Editorial not found !!!");
        }

        var editorialDelete = await _editorialRepository.DeleteEditorial(id);
        if (!editorialDelete)
        {
            return new ApiResult<bool>(false, "Failed to delete editorial !!!");
        }

        return new ApiSuccessResult<bool>(true);
    }
}