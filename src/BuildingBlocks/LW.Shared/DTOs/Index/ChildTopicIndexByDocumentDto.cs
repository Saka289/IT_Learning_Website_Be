namespace LW.Shared.DTOs.Index;

public class ChildTopicIndexByDocumentDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public IEnumerable<LessonIndexByDocumentDto> Lessons { get; set; }
}