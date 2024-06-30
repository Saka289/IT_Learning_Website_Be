namespace LW.Shared.DTOs.Index;

public class DocumentIndexByLessonParentTopicDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public TopicIndexByLessonParentTopicDto Topic { get; set; }
}