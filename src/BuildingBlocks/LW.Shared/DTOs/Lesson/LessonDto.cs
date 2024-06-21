namespace LW.Shared.DTOs.Lesson;

public class LessonDto
{
    public int Id { get; set;  }
    public string Title { get; set; }
    public bool IsActive { get; set; }
    public string Content { get; set; }
    public string FilePath { get; set; }
    public string UrlDownload { get; set; }
    public int TopicId { get; set; }
}
