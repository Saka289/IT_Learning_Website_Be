using LW.Shared.Enums;

namespace LW.Shared.DTOs.Grade;

public class GradeProblemDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Difficulty { get; set; }
    public string DifficultyName { get; set; }
    public bool IsActive { get; set; }
    public int? TopicId { get; set; }
    public int? LessonId { get; set; }
    public int? GradeId { get; set; }
    public EStatusProblem Status { get; set; }
}