using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Lesson;

public class SearchLessonDto : SearchRequestValue
{
    public int? TopicId { get; set; }
    public bool? Status { get; set; }
}