using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Quiz;

public class SearchQuizDto : SearchRequestValue
{
    public int? TopicId { get; set; }
    public int? LessonId { get; set; }
    public bool Custom { get; set; } = false;
    public ETypeQuiz? Type { get; set; }
}