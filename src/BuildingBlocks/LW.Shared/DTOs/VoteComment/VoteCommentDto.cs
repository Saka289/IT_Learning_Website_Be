namespace LW.Shared.DTOs.VoteComment;

public class VoteCommentDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
   
    public int PostCommentId { get; set; }
    
    public bool IsCorrectVote { get; set; }
    
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}