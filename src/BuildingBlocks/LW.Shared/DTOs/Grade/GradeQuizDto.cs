namespace LW.Shared.DTOs.Grade;

public class GradeQuizDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int TypeId { get; set; }
    public string TypeName { get; set; }
    public decimal Score { get; set; }
    public bool IsActive { get; set; }
    public int? TopicId { get; set; }
    public int? LessonId { get; set; }
}