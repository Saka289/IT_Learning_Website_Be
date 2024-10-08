﻿using LW.Data.Entities;
using LW.Shared.DTOs.Quiz;
using LW.Shared.DTOs.QuizAnswer;
using LW.Shared.DTOs.QuizQuestion;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;

namespace LW.Services.QuizQuestionServices;

public interface IQuizQuestionService
{
    Task<ApiResult<IEnumerable<QuizQuestionDto>>> GetAllQuizQuestion(bool? status);
    Task<ApiResult<PagedList<QuizQuestionDto>>> GetAllQuizQuestionPagination(SearchAllQuizQuestionDto searchAllQuizQuestionDto);
    Task<ApiResult<IEnumerable<object>>> GetAllQuizQuestionByQuizId(SearchQuizQuestionDto searchQuizQuestionDto);
    Task<ApiResult<QuizQuestionDto>> GetQuizQuestionById(int id);
    Task<ApiResult<QuizQuestionDto>> CreateQuizQuestion(QuizQuestionCreateDto quizQuestionCreateDto);
    Task<ApiResult<QuizQuestionDto>> UpdateQuizQuestion(QuizQuestionUpdateDto quizQuestionUpdateDto);
    Task<ApiResult<bool>> CreateRangeQuizQuestion(IEnumerable<QuizQuestionCreateDto> quizQuestionsCreateDto);
    Task<ApiResult<bool>> UpdateRangeQuizQuestion(IEnumerable<QuizQuestionUpdateDto> quizQuestionsUpdateDto);
    Task<ApiResult<bool>> UpdateStatusQuizQuestion(int id);
    Task<ApiResult<bool>> DeleteQuizQuestion(int id);
    public Task<byte[]> ExportExcel(int checkData = 1, string? Ids = null);
    public Task<ApiResult<QuizQuestionImportParentDto>> ImportExcel(IFormFile formFile, int quizId);
    public Task<ApiResult<bool>> ImportDatabase(string idCache, int quizId);
}