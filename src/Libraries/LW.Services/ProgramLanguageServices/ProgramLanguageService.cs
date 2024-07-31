using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.ProgramLanguageRepositories;
using LW.Shared.DTOs.ProgramLanguage;
using LW.Shared.SeedWork;

namespace LW.Services.ProgramLanguageServices;

public class ProgramLanguageService : IProgramLanguageService
{
    private readonly IProgramLanguageRepository _programLanguageRepository;
    private readonly IMapper _mapper;

    public ProgramLanguageService(IProgramLanguageRepository programLanguageRepository, IMapper mapper)
    {
        _programLanguageRepository = programLanguageRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<IEnumerable<ProgramLanguageDto>>> GetAllProgramLanguage()
    {
        var languageList = await _programLanguageRepository.GetAllProgramLanguage();
        if (!languageList.Any())
        {
            return new ApiResult<IEnumerable<ProgramLanguageDto>>(false, "Program Language not found !!!");
        }

        var result = _mapper.Map<IEnumerable<ProgramLanguageDto>>(languageList);
        return new ApiSuccessResult<IEnumerable<ProgramLanguageDto>>(result);
    }

    public async Task<ApiResult<ProgramLanguageDto>> GetProgramLanguageById(int id)
    {
        var language = await _programLanguageRepository.GetProgramLanguageById(id);
        if (language is null)
        {
            return new ApiResult<ProgramLanguageDto>(false, "Program language not found !!!");
        }

        var result = _mapper.Map<ProgramLanguageDto>(language);
        return new ApiSuccessResult<ProgramLanguageDto>(result);
    }

    public async Task<ApiResult<ProgramLanguageDto>> CreateProgramLanguage(
        ProgramLanguageCreateDto programLanguageCreateDto)
    {
        if (programLanguageCreateDto is null)
        {
            return new ApiResult<ProgramLanguageDto>(false, "Program language is null !!!");
        }

        var language = _mapper.Map<ProgramLanguage>(programLanguageCreateDto);
        var languageCreate = await _programLanguageRepository.CreateProgramLanguage(language);
        var result = _mapper.Map<ProgramLanguageDto>(languageCreate);
        return new ApiSuccessResult<ProgramLanguageDto>(result);
    }

    public async Task<ApiResult<ProgramLanguageDto>> UpdateProgramLanguage(
        ProgramLanguageUpdateDto programLanguageUpdateDto)
    {
        var language = await _programLanguageRepository.GetProgramLanguageById(programLanguageUpdateDto.Id);
        if (language is null)
        {
            return new ApiResult<ProgramLanguageDto>(false, "Program Languages not found !!!");
        }

        var languageMapper = _mapper.Map(programLanguageUpdateDto, language);
        var languageUpdate = await _programLanguageRepository.UpdateProgramLanguage(languageMapper);
        var result = _mapper.Map<ProgramLanguageDto>(languageUpdate);
        return new ApiSuccessResult<ProgramLanguageDto>(result);
    }

    public async Task<ApiResult<bool>> UpdateStatusProgramLanguage(int id)
    {
        var language = await _programLanguageRepository.GetProgramLanguageById(id);
        if (language is null)
        {
            return new ApiResult<bool>(false, "Program Languages not found !!!");
        }

        language.IsActive = !language.IsActive;
        await _programLanguageRepository.UpdateProgramLanguage(language);
        return new ApiSuccessResult<bool>(true);
    }

    public async Task<ApiResult<bool>> DeleteProgramLanguage(int id)
    {
        var language = await _programLanguageRepository.GetProgramLanguageById(id);
        if (language is null)
        {
            return new ApiResult<bool>(false, "Program Languages not found !!!");
        }

        var languageDelete = await _programLanguageRepository.DeleteProgramLanguage(id);
        if (!languageDelete)
        {
            return new ApiResult<bool>(false, "Failed to delete program language !!!");
        }

        return new ApiSuccessResult<bool>(true);
    }
}