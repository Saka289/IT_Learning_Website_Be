using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Subject :EntityAuditBase<int>
{
    [Required]
    public string Name { get; set; }
    [Required]
    public bool Active { get; set; }
    [Required]
    public int GradeId { get; set; }
    [ForeignKey("GradeId")]
    public virtual Grade Grade { get; set; }
    
    //
    public ICollection<Document> Documents { get; set; }
}