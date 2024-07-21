namespace LW.Shared.DTOs.Quiz;

public class QuizDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string KeyWord { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public decimal Score { get; set; }
    public bool IsActive { get; set; }
    public int? TopicId { get; set; }
    public string? TopicTitle { get; set; }
    public int? LessonId { get; set; }
    public string? LessonTitle { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
    public string CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
}