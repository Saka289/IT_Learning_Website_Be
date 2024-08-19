namespace LW.Shared.DTOs.Index;

public class TopicIndexByLessonParentTopicDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public ChildTopicIndexByLessonDto ParentTopic { get; set; }
    public IEnumerable<LessonIndexByLessonDto> Lessons { get; set; }
}