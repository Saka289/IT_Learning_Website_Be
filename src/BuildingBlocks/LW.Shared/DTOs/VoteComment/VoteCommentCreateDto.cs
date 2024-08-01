namespace LW.Shared.DTOs.VoteComment;

public class VoteCommentCreateDto
{
    public string? UserId { get; set; }
   
    public int PostCommentId { get; set; }
    
    public bool IsCorrectVote { get; set; }
}