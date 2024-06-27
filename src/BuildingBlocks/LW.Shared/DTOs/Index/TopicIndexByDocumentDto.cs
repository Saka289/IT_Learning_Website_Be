using LW.Shared.DTOs.Index;

namespace LW.Shared.DTOs.Index;

public class TopicIndexByDocumentDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public IEnumerable<LessonIndexByDocumentDto> Lessons { get; set; }
}