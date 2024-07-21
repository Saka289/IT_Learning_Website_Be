using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.QuizQuestionRelationRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Shared.DTOs.QuizQuestionRelation;
using LW.Shared.SeedWork;
using Serilog;

namespace LW.Services.QuizQuestionRelationServices;

public class QuizQuestionRelationService : IQuizQuestionRelationService
{
    private readonly IQuizQuestionRelationRepository _quizQuestionRelationRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public QuizQuestionRelationService(IQuizQuestionRelationRepository quizQuestionRelationRepository, IMapper mapper,
        IQuizRepository quizRepository, ILogger logger)
    {
        _quizQuestionRelationRepository = quizQuestionRelationRepository;
        _mapper = mapper;
        _quizRepository = quizRepository;
        _logger = logger;
    }

    public async Task<ApiResult<bool>> CreateQuizQuestionRelation(QuizQuestionRelationCreateDto quizQuestionRelationCreateDto)
    {
        var quizEntity = await _quizRepository.GetQuizById(quizQuestionRelationCreateDto.QuizId);
        if (quizEntity is null)
        {
            return new ApiResult<bool>(false, "Quiz not found !!!");
        }

        // var quizQuestionRelation = new List<QuizQuestionRelation>();
        // foreach (var item in quizQuestionRelationCreateDto.QuizQuestionIds)
        // {
        //     quizQuestionRelation.Add(new()
        //     {
        //         QuizId = quizQuestionRelationCreateDto.QuizId,
        //         QuizQuestionId = item
        //     });
        // }
        var quizQuestionRelation = quizQuestionRelationCreateDto.QuizQuestionIds.Select(quizQuestionId =>
            new QuizQuestionRelation
            {
                QuizId = quizQuestionRelationCreateDto.QuizId,
                QuizQuestionId = quizQuestionId
            }).ToList();
        var create = await _quizQuestionRelationRepository.CreateRangeQuizQuestionRelation(quizQuestionRelation);
        if (!create)
        {
            return new ApiResult<bool>(false, "Failed to create QuizQuestionRelation !!!");
        }

        return new ApiSuccessResult<bool>(create);
    }

    public async Task<ApiResult<bool>> UpdateQuizQuestionRelation(QuizQuestionRelationUpdateDto quizQuestionRelationUpdateDto)
    {
        var quizEntity = await _quizRepository.GetQuizById(quizQuestionRelationUpdateDto.QuizId);
        if (quizEntity is null)
        {
            return new ApiResult<bool>(false, "Quiz not found !!!");
        }

        var listRelation = await _quizQuestionRelationRepository.GetAllQuizQuestionRelationByQuizId(quizQuestionRelationUpdateDto.QuizId);
        var deleteRelation = await _quizQuestionRelationRepository.DeleteRangeQuizQuestionRelation(listRelation);
        if (!deleteRelation)
        {
            return new ApiResult<bool>(false, "Failed to update QuizQuestionRelation !!!");
        }
        var quizQuestionRelation = quizQuestionRelationUpdateDto.QuizQuestionIds.Select(quizQuestionId =>
            new QuizQuestionRelation
            {
                QuizId = quizQuestionRelationUpdateDto.QuizId,
                QuizQuestionId = quizQuestionId
            }).ToList();
        var update = await _quizQuestionRelationRepository.CreateRangeQuizQuestionRelation(quizQuestionRelation);
        if (!update)
        {
            return new ApiResult<bool>(false, "Failed to update QuizQuestionRelation !!!");
        }

        return new ApiSuccessResult<bool>(update);
    }

    public async Task<ApiResult<bool>> DeleteQuizQuestionRelation(int id)
    {
        var quizQuestionRelation = await _quizQuestionRelationRepository.GetQuizQuestionRelationById(id);
        if (quizQuestionRelation is null)
        {
            return new ApiResult<bool>(false, "Quiz Question Relation not found !!!");
        }

        var delete = await _quizQuestionRelationRepository.DeleteQuizQuestionRelation(id);
        if (!delete)
        {
            return new ApiResult<bool>(false, "Failed to delete QuizQuestionRelation !!!");
        }
        return new ApiSuccessResult<bool>(delete);
    }

    public async Task<ApiResult<bool>> DeleteRangeQuizQuestionRelation(IEnumerable<int> ids)
    {
        var listRelations = new List<QuizQuestionRelation>();
        foreach (var item in ids)
        {
            var quizQuestionRelation = await _quizQuestionRelationRepository.GetQuizQuestionRelationById(item);
            if (quizQuestionRelation is not null)
            {
                listRelations.Add(quizQuestionRelation);
            }
            _logger.Information($"Not found with id {item}");
        }
        
        var delete = await _quizQuestionRelationRepository.DeleteRangeQuizQuestionRelation(listRelations);
        if (!delete)
        {
            return new ApiResult<bool>(false, "Failed to delete QuizQuestionRelation !!!");
        }
        return new ApiSuccessResult<bool>(delete);
    }
}