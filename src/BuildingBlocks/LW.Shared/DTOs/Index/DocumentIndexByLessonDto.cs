namespace LW.Shared.DTOs.Index;

public class DocumentIndexByLessonDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public TopicIndexByLessonDto Topic { get; set; }
}