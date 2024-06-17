using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Document:EntityAuditBase<int>
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string FilePath { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public int SubjectId { get; set; }
    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; }
    [Required]
    public bool  IsActive { get; set; }
    public ICollection<Topic> Topics { get; set; }

}