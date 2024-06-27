namespace LW.Shared.DTOs.Index;

public class TopicIndexByTopicDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public IEnumerable<LessonIndexByTopicDto> Lessons { get; set; }
}