﻿using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs.Lesson;
using LW.Shared.SeedWork;

namespace LW.Services.LessonService;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IElasticSearchService<Lesson, int> _elasticSearchService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IMapper _mapper;

    public LessonService(ILessonRepository lessonRepository, ITopicRepository topicRepository, IMapper mapper,
        IElasticSearchService<Lesson, int> elasticSearchService, ICloudinaryService cloudinaryService)
    {
        _lessonRepository = lessonRepository;
        _topicRepository = topicRepository;
        _mapper = mapper;
        _elasticSearchService = elasticSearchService;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ApiResult<IEnumerable<LessonDto>>> GetAllLesson()
    {
        var lessonList = await _lessonRepository.GetAllLesson();
        if (lessonList == null)
        {
            return new ApiResult<IEnumerable<LessonDto>>(false, "Lesson is null !!!");
        }

        var result = _mapper.Map<IEnumerable<LessonDto>>(lessonList);
        return new ApiSuccessResult<IEnumerable<LessonDto>>(result);
    }

    public async Task<ApiResult<LessonDto>> GetLessonById(int id)
    {
        var lessonEntity = await _lessonRepository.GetLessonById(id);
        if (lessonEntity == null)
        {
            return new ApiResult<LessonDto>(false, "Lesson is null !!!");
        }

        var result = _mapper.Map<LessonDto>(lessonEntity);
        return new ApiSuccessResult<LessonDto>(result);
    }

    public async Task<ApiResult<IEnumerable<LessonDto>>> SearchByLesson(SearchLessonDto searchLessonDto)
    {
        var lessonEntity =
            await _elasticSearchService.SearchDocumentAsync(ElasticConstant.ElasticLessons, searchLessonDto);
        if (lessonEntity is null)
        {
            return new ApiResult<IEnumerable<LessonDto>>(false, $"Lesson not found by {searchLessonDto.Key} !!!");
        }

        var result = _mapper.Map<IEnumerable<LessonDto>>(lessonEntity);
        return new ApiSuccessResult<IEnumerable<LessonDto>>(result);
    }

    public async Task<ApiResult<LessonDto>> CreateLesson(LessonCreateDto lessonCreateDto)
    {
        var topicEntity = await _topicRepository.GetTopicById(lessonCreateDto.TopicId);
        if (topicEntity is null)
        {
            return new ApiResult<LessonDto>(false, "TopicId not found !!!");
        }

        var lessonEntity = _mapper.Map<Lesson>(lessonCreateDto);
        var filePath =
            await _cloudinaryService.CreateFileAsync(lessonCreateDto.FilePath, CloudinaryConstant.FolderLessonFile);
        lessonEntity.KeyWord = lessonCreateDto.Title.RemoveDiacritics();
        lessonEntity.FilePath = filePath.Url;
        lessonEntity.PublicId = filePath.PublicId;
        lessonEntity.UrlDownload = filePath.UrlDownload;
        await _lessonRepository.CreateLesson(lessonEntity);
        await _lessonRepository.SaveChangesAsync();
        await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticLessons, lessonEntity, g => g.Id);
        var result = _mapper.Map<LessonDto>(lessonEntity);
        return new ApiSuccessResult<LessonDto>(result);
    }

    public async Task<ApiResult<LessonDto>> UpdateLesson(LessonUpdateDto lessonUpdateDto)
    {
        var topicEntity = await _topicRepository.GetTopicById(lessonUpdateDto.TopicId);
        if (topicEntity is null)
        {
            return new ApiResult<LessonDto>(false, "TopicId not found !!!");
        }

        var lessonEntity = await _lessonRepository.GetByIdAsync(lessonUpdateDto.Id);
        if (lessonEntity is null)
        {
            return new ApiResult<LessonDto>(false, "Lesson not found !!!");
        }

        var model = _mapper.Map(lessonUpdateDto, lessonEntity);
        model.KeyWord = lessonUpdateDto.Title.RemoveDiacritics();
        if (lessonUpdateDto.FilePath != null && lessonUpdateDto.FilePath.Length > 0)
        {
            var filePath = await _cloudinaryService.UpdateFileAsync(lessonEntity.PublicId, lessonUpdateDto.FilePath);
            model.FilePath = filePath.Url;
            model.PublicId = filePath.PublicId;
            model.UrlDownload = filePath.UrlDownload;
            var updateLessonFile = await _lessonRepository.UpdateLesson(model);
            await _lessonRepository.SaveChangesAsync();
            await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticLessons, updateLessonFile,
                lessonUpdateDto.Id);
            var resultFile = _mapper.Map<LessonDto>(updateLessonFile);
            return new ApiSuccessResult<LessonDto>(resultFile);
        }

        var updateLesson = await _lessonRepository.UpdateLesson(model);
        await _lessonRepository.SaveChangesAsync();
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticLessons, updateLesson,
            lessonUpdateDto.Id);
        var result = _mapper.Map<LessonDto>(updateLesson);
        return new ApiSuccessResult<LessonDto>(result);
    }

    public async Task<ApiResult<bool>> UpdateLessonStatus(int id)
    {
        var lessonEntity = await _lessonRepository.GetLessonById(id);
        if (lessonEntity is null)
        {
            return new ApiResult<bool>(false, "Lesson not found !!!");
        }

        lessonEntity.IsActive = !lessonEntity.IsActive;
        await _lessonRepository.UpdateLesson(lessonEntity);
        await _lessonRepository.SaveChangesAsync();
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticLessons, lessonEntity, id);
        return new ApiSuccessResult<bool>(true, "Lesson update successfully !!!");
    }

    public async Task<ApiResult<bool>> DeleteLesson(int id)
    {
        var lessonEntity = await _lessonRepository.GetLessonById(id);
        if (lessonEntity is null)
        {
            return new ApiResult<bool>(false, "Lesson not found !!!");
        }

        var lesson = await _lessonRepository.DeleteLesson(id);
        if (!lesson)
        {
            return new ApiResult<bool>(false, "Failed Delete Lesson not found !!!");
        }
        
        await _cloudinaryService.DeleteFileAsync(lessonEntity.PublicId);
        await _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticLessons, id);

        return new ApiSuccessResult<bool>(true, "Delete Lesson Successfully !!!");
    }
}