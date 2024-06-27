using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Document : EntityAuditBase<int>
{
    [Required] 
    public string Title { get; set; }
    [Required] 
    public string Description { get; set; }
    [Required] 
    public string KeyWord { get; set; }
    [Required]
    public bool IsActive { get; set; }
    [Required] 
    public int GradeId { get; set; }
    [ForeignKey(nameof(GradeId))] 
    public virtual Grade Grade { get; set; }
    public virtual ICollection<Topic> Topics { get; set; }
    public virtual ICollection<CommentDocument> CommentDocuments { get; set; }
}