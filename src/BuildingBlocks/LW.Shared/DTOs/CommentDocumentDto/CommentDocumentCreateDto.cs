namespace LW.Shared.DTOs.CommentDocumentDto;

public class CommentDocumentCreateDto
{
    public string Note { get; set; }
    public int Rating { get; set; }
    public int? ParentId { get; set; }
    public int DocumentId { get; set; }
    public string UserId { get; set; }
}