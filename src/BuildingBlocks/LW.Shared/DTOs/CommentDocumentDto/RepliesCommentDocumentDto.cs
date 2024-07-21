namespace LW.Shared.DTOs.CommentDocumentDto;

public class RepliesCommentDocumentDto
{
    public int Id { get; set; }
    public string Note { get; set; }
    public int Rating { get; set; }
    public int? ParentId { get; set; }
    public int DocumentId { get; set; }
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string Avatar { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
    public string CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
}