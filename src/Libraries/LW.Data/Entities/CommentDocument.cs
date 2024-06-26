using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class CommentDocument : EntityAuditBase<int>
{
    [Required] 
    public string Note { get; set; }
    [Required] 
    public int Rating { get; set; }
    public int? ParentId { get; set; }
    [Required] 
    public int DocumentId { get; set; }
    [ForeignKey(nameof(DocumentId))]
    public virtual Document Document { get; set; }
    [Required] 
    public string UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser ApplicationUser { get; set; }
    [ForeignKey(nameof(ParentId))]
    public virtual CommentDocument ParentComment { get; set; }
    public virtual ICollection<CommentDocument> Replies { get; set; }
}