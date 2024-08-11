using LW.Shared.Enums;

namespace LW.Shared.DTOs.Problem;

public class ProblemUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public EDifficulty Difficulty { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<string> tagValues { get; set; }
    public int? TopicId { get; set; }
    public int? LessonId { get; set; }
}