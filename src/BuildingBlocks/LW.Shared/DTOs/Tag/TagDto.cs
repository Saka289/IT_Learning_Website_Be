namespace LW.Shared.DTOs.Tag;

public class TagDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? KeyWord { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}