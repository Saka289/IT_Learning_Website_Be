using LW.Shared.Enums;

namespace LW.Shared.DTOs.Problem;

public class ProblemCreateDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public EDifficulty Difficulty { get; set; }
    public bool IsActive { get; set; }
    public int? TopicId { get; set; }
    public int? LessonId { get; set; }
}