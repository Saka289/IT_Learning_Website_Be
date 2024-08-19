namespace LW.Shared.DTOs.PostComment;

public class PostCommentUpdateDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int PostId { get; set; }
    public string UserId { get; set; }
    public int? ParentId { get; set; }
}