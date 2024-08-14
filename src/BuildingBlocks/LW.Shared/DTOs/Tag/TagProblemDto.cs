namespace LW.Shared.DTOs.Tag;

public class TagProblemDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string KeyWord { get; set; }
    public string Description { get; set; }
    public int Difficulty { get; set; }
    public bool IsActive { get; set; }
    public int? TopicId { get; set; }
    public int? LessonId { get; set; }
    public int? GradeId { get; set; }
}