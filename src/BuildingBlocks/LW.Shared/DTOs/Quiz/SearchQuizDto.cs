using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Quiz;

public class SearchQuizDto : SearchRequestParameters
{
    public override string? Key { get; set; } = "keyWord";
    public int? TopicId { get; set; }
    public int? LessonId { get; set; }
}