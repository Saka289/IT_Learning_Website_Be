using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class VoteComment : EntityAuditBase<int>
{
    [Required]
    public string UserId { get; set; }
    [Required]
    public int PostCommentId { get; set; }
    public bool IsCorrectVote { get; set; }
    [ForeignKey(nameof(PostCommentId))]
    public virtual PostComment PostComment { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser ApplicationUser { get; set; }
}