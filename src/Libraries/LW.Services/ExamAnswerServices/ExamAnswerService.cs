using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.ExamAnswerRepositories;
using LW.Data.Repositories.ExamCodeRepositories;
using LW.Data.Repositories.ExamRepositories;
using LW.Services.ExamCodeServices;
using LW.Shared.DTOs.ExamAnswer;
using LW.Shared.SeedWork;

namespace LW.Services.ExamAnswerServices;

public class ExamAnswerService : IExamAnswerService
{
    private readonly IMapper _mapper;
    private readonly IExamAnswerRepository _examAnswerRepository;
    private readonly IExamRepository _examRepository;
    private readonly IExamCodeRepository _examCodeRepository;
    public ExamAnswerService(IMapper mapper, IExamAnswerRepository examAnswerRepository, IExamRepository examRepository, IExamCodeRepository examCodeRepository)
    {
        _mapper = mapper;
        _examAnswerRepository = examAnswerRepository;
        _examRepository = examRepository;
        _examCodeRepository = examCodeRepository;
    }
    public async Task<ApiResult<IEnumerable<ExamAnswerDto>>> GetAllExamAnswer()
    {
        var answers = await _examAnswerRepository.GetAllExamAnswer();
        if (answers.Count() == 0)
        {
            return new ApiResult<IEnumerable<ExamAnswerDto>>(false, "Not Found");
        }

        var result = _mapper.Map<IEnumerable<ExamAnswerDto>>(answers);
        return new ApiResult<IEnumerable<ExamAnswerDto>>(true, result, "Get successfully");
    }

    public async Task<ApiResult<ExamAnswerDto>> GetExamAnswerById(int id)
    {
        var answer = await _examAnswerRepository.GetExamAnswerById(id);
        if (answer == null)
        {
            return new ApiResult<ExamAnswerDto>(false, "Not found");
        }

        var result = _mapper.Map<ExamAnswerDto>(answer);
        return new ApiResult<ExamAnswerDto>(true, result, "Get By Id Successfully");
    }

    public async Task<ApiResult<IEnumerable<ExamAnswerDto>>> GetExamAnswerByExamCodeId(int examCodeId)
    {
        var answers = await _examAnswerRepository.GetAllExamAnswerByExamCodeId(examCodeId);
        if (answers.Count() == 0)
        {
            return new ApiResult<IEnumerable<ExamAnswerDto>>(true, "Not Found");
        }

        var result = _mapper.Map<IEnumerable<ExamAnswerDto>>(answers);
        return new ApiResult<IEnumerable<ExamAnswerDto>>(true, result, "Get list answer by examId successfully");
    }

    public async Task<ApiResult<ExamAnswerDto>> CreateExamAnswer(ExamAnswerCreateDto examAnswerCreateDto)
    {
        var examCode = await _examCodeRepository.GetExamCodeById(examAnswerCreateDto.ExamCodeId);
        if (examCode == null)
        {
            return new ApiResult<ExamAnswerDto>(false, $"Not find exam code with examId = {examAnswerCreateDto.ExamCodeId}");
        }
        // check number question is existed
        var answerExist =
            await _examAnswerRepository.GetExamAnswerByNumberOfQuestion(examAnswerCreateDto.NumberOfQuestion);
        if (answerExist != null)
        {
            return new ApiResult<ExamAnswerDto>(false, $"An answer already exists with number of question = {examAnswerCreateDto.NumberOfQuestion}");
        }
        
        var answer = _mapper.Map<ExamAnswer>(examAnswerCreateDto);
        await _examAnswerRepository.CreateExamAnswer(answer);
        var result = _mapper.Map<ExamAnswerDto>(answer);
        return new ApiResult<ExamAnswerDto>(true, result, "Create answer for exam code successfully");
    }

    public async Task<ApiResult<bool>> CreateRangeExamAnswer(ExamAnswerCreateRangeDto examAnswerCreateRangeDtos)
    {
        var exam = await _examCodeRepository.GetExamCodeById(examAnswerCreateRangeDtos.ExamCodeId);
        if (exam == null)
        {
            return new ApiResult<bool>(false, "ExamCode not found");
        }

        var listExamAnswer = examAnswerCreateRangeDtos.AnswerDtos.Select(dto => new ExamAnswer()
        {
            ExamCodeId = examAnswerCreateRangeDtos.ExamCodeId,
            NumberOfQuestion = dto.NumberOfQuestion,
            Answer = dto.Answer
        }).ToList();
        
        await _examAnswerRepository.CreateRangeExamAnswer(listExamAnswer);
        return new ApiResult<bool>(true, $"Create Range Answer For ExamCode with id= {examAnswerCreateRangeDtos.ExamCodeId} Successfully");
    }

    public async Task<ApiResult<bool>> UpdateRangeExamAnswer(ExamAnswerUpdateRangeDto examAnswers)
    {
        var examCode = await _examCodeRepository.GetExamCodeById(examAnswers.ExamCodeId);
        if (examCode == null)
        {
            return new ApiResult<bool>(false, "ExamCode not found");
        }
        var listExamAnswer = examAnswers.AnswerUpdateDtos.Select(dto => new ExamAnswer()
        {
            Id = dto.Id,
            ExamCodeId = examAnswers.ExamCodeId,
            NumberOfQuestion = dto.NumberOfQuestion,
            Answer = dto.Answer
        }).ToList();
        await _examAnswerRepository.UpdateRangeExamAnswer(listExamAnswer);
        return new ApiResult<bool>(true, $"Create Range Answer For ExamCode with id= {examAnswers.ExamCodeId} Successfully");
    }

    public async  Task<ApiResult<ExamAnswerDto>> UpdateExamAnswer(ExamAnswerUpdateDto examAnswerUpdateDto)
    {
        var answerCheck = await _examAnswerRepository.GetExamAnswerById(examAnswerUpdateDto.Id);
        if (answerCheck == null)
        {
            return new ApiResult<ExamAnswerDto>(false, "Not Found");
        }
        var exam = await _examCodeRepository.GetExamCodeById(examAnswerUpdateDto.ExamCodeId);
        if (exam == null)
        {
            return new ApiResult<ExamAnswerDto>(false, "Not Found ExamCode");
        }

        var answer = _mapper.Map<ExamAnswer>(examAnswerUpdateDto);
        await _examAnswerRepository.UpdateExamAnswer(answer);
        var result = _mapper.Map<ExamAnswerDto>(answer);
        return new ApiResult<ExamAnswerDto>(true, result, "Update answer for examcode successfully");
    }

    public async Task<ApiResult<bool>> DeleteExamAnswer(int id)
    {
        var answer = await _examAnswerRepository.GetExamAnswerById(id);
        if (answer == null)
        {
            return new ApiResult<bool>(false, "Not found");
        }

        await _examAnswerRepository.DeleteExamAnswer(id);
        return new ApiResult<bool>(true, "Delete answer successfully");
    }
}