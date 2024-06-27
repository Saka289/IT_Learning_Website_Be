using LW.Shared.DTOs.Index;
using LW.Shared.SeedWork;

namespace LW.Services.IndexServices;

public interface IIndexService
{
    Task<ApiResult<DocumentIndexByDocumentDto>> GetAllDocumentIndex(int documentId);
    Task<ApiResult<DocumentIndexByTopicDto>> GetAllTopicIndex(int topicId);
    Task<ApiResult<DocumentIndexByLessonDto>> GetAllLessonIndex(int topicId);
}