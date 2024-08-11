using LW.Shared.Enums;

namespace LW.Shared.DTOs.Tag;

public class TagQuizDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string KeyWord { get; set; }
    public string Description { get; set; }
    public ETypeQuiz Type { get; set; }
    public decimal Score { get; set; }
    public bool IsActive { get; set; }
    public int? TopicId { get; set; }
    public int? LessonId { get; set; }
    public int TotalQuestion { get; set; }
}