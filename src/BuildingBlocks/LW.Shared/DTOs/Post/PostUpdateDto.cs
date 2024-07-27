namespace LW.Shared.DTOs.Post;

public class PostUpdateDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string UserId { get; set; }
    public int GradeId { get; set; }
}