using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Grade : EntityAuditBase<int>
{
    [Required] 
    public string Name { get; set; }
    [Required] 
    public bool Active { get; set; }
    [Required] 
    public int LevelId { get; set; }
    [ForeignKey("LevelId")] 
    public virtual Level Level { get; set; }
    public ICollection<Subject> Subjects { get; set; }
}