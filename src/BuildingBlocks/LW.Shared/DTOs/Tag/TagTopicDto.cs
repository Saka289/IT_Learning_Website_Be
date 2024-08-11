namespace LW.Shared.DTOs.Tag;

public class TagTopicDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string KeyWord { get; set; }
    public string Objectives { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int? ParentId { get; set; }
    public int DocumentId { get; set; }
    public IEnumerable<TagTopicDto> ChildTopics { get; set; }
}