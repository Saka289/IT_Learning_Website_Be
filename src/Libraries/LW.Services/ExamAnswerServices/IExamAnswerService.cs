﻿using LW.Shared.DTOs;
using LW.Shared.DTOs.ExamAnswer;
using LW.Shared.SeedWork;

namespace LW.Services.ExamAnswerServices;

public interface IExamAnswerService
{
    Task<ApiResult<IEnumerable<ExamAnswerDto>>> GetAllExamAnswer();
    Task<ApiResult<ExamAnswerDto>> GetExamAnswerById(int id);
    Task<ApiResult<IEnumerable<ExamAnswerDto>>> GetExamAnswerByExamCodeId(int examCodeId);
    Task<ApiResult<ExamAnswerDto>> CreateExamAnswer(ExamAnswerCreateDto examAnswerCreateDto);
    
    Task<ApiResult<bool>> CreateRangeExamAnswer(ExamAnswerCreateRangeDto examAnswerCreateRangeDtos);
    Task<ApiResult<bool>> UpdateRangeExamAnswer(ExamAnswerUpdateRangeDto examAnswers);
    Task<ApiResult<ExamAnswerDto>> UpdateExamAnswer(ExamAnswerUpdateDto examAnswerUpdateDto);
    Task<ApiResult<bool>> DeleteExamAnswer(int id);
}