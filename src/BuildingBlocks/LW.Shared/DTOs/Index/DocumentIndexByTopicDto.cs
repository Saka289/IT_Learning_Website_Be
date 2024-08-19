namespace LW.Shared.DTOs.Index;

public class DocumentIndexByTopicDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public TopicIndexByTopicDto Topic { get; set; }
}