namespace LW.Shared.DTOs.Index;

public class DocumentIndexByTopicParentDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public TopicIndexByTopicParentDto Topic { get; set; }
}