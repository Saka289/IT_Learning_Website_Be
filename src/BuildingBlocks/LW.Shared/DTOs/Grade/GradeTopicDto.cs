namespace LW.Shared.DTOs.Grade;

public class GradeTopicDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string Objectives { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int? ParentId { get; set; }
    public int DocumentId { get; set; }
    public IEnumerable<GradeTopicDto> ChildTopics { get; set; }
    public IEnumerable<GradeProblemDto> Problems { get; set; }
    public IEnumerable<GradeQuizDto> Quizzes { get; set; }
    public IEnumerable<GradeLessonDto> Lessons { get; set; }
}