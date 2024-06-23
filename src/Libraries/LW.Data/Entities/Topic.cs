using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Topic : EntityAuditBase<int>
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string KeyWord { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string Objectives { get; set; }
    [Required]
    public bool IsActive { get; set; }
    [Required]
    public int DocumentId { get; set; }
    [ForeignKey(nameof(DocumentId))]
    public virtual Document Document { get; set; }
    public virtual ICollection<Lesson> Lessons { get; set; }
}