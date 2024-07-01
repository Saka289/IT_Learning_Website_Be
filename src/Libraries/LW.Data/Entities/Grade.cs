using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Grade : EntityAuditBase<int>
{
    [Required] 
    public string Title { get; set; }
    [Required] 
    public string KeyWord { get; set; }
    [Required] 
    public bool IsActive { get; set; }
    [Required] 
    public int LevelId { get; set; }
    [ForeignKey(nameof(LevelId))] 
    public virtual Level Level { get; set; }
    public virtual ICollection<Document> Documents { get; set; }
    public virtual ICollection<UserGrade> UserGrades { get; set; }
}