using LW.Shared.DTOs.Index;
using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Services.IndexServices;

public interface IIndexService
{
    Task<ApiResult<DocumentIndexByDocumentDto>> GetAllDocumentIndex(int documentId);
    Task<ApiResult<DocumentIndexByTopicDto>> GetAllTopicIndex(int topicId);
    Task<ApiResult<ETopicIndex>> CheckTopicById(int topicId);
    Task<ApiResult<DocumentIndexByTopicParentDto>> GetAllTopicParentIndex(int topicParentId);
    Task<ApiResult<ELessonIndex>> CheckLessonById(int topicId);
    Task<ApiResult<DocumentIndexByLessonDto>> GetAllLessonIndex(int lessonId);
    Task<ApiResult<DocumentIndexByLessonParentTopicDto>> GetAllLessonParentTopicIndex(int lessonId);
}