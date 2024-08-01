namespace LW.Shared.DTOs.VoteComment;

public class VoteCommentUpdateDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public int PostCommentId { get; set; }
    public bool IsCorrectVote { get; set; }
}