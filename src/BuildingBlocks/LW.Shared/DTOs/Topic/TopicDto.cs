namespace LW.Shared.DTOs.Topic;

public class TopicDto
{
    public int Id { get; set; }
    
    public string? Title { get; set; }
    
    public string? KeyWord { get; set; }
    
    public string? Description { get; set; }
    
    public bool IsActive { get; set; }
}