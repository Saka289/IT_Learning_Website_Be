namespace LW.Shared.DTOs.PostComment;

public class PostCommentReplyDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int PostId { get; set; }
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string Avatar { get; set; }
    public int? ParentId { get; set; }
    public int? CorrectVote { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
    public string CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
}