using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Quiz;

public class SearchQuizDto : SearchRequestValue
{
    public int? TopicId { get; set; }
    public int? LessonId { get; set; }
    public ECustomQuiz Custom { get; set; } = ECustomQuiz.All;
    public ETypeQuiz? Type { get; set; }
    public bool? Status { get; set; }
}