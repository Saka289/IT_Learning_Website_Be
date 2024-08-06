namespace LW.Shared.DTOs.CommentDocument;

public class CommentDocumentUpdateDto
{
    public int Id { get; set; }
    public string Note { get; set; }
    public int Rating { get; set; }
    public int? ParentId { get; set; }
    public int DocumentId { get; set; }
    public string UserId { get; set; }
}