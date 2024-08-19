namespace LW.Shared.DTOs.PostComment;

public class PostCommentCreateDto
{
    public string Content { get; set; }
    public int PostId { get; set; }
    public string UserId { get; set; }
    public int? ParentId { get; set; }
}