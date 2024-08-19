namespace LW.Shared.DTOs.Tag;

public class TagLessonDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string KeyWord { get; set; }
    public bool IsActive { get; set; }
    public string Content { get; set; }
    public int TopicId { get; set; }
}