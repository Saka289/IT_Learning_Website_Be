using LW.Shared.Enums;

namespace LW.Shared.DTOs.Quiz;

public class QuizCreateDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public ETypeQuiz Type { get; set; }
    public decimal Score { get; set; }
    public bool IsActive { get; set; } = true;
    public int? TopicId { get; set; }
    public int? LessonId { get; set; }
}