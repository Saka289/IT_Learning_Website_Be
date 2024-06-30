namespace LW.Shared.DTOs.Index;

public class TopicIndexByTopicParentDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public ChildTopicIndexByTopicDto ParentTopic { get; set; }
    public IEnumerable<LessonIndexByTopicDto> Lessons { get; set; }
}