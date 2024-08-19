using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class UserGrade : EntityBase<int>
{
    [Required]
    public int GradeId { get; set; }
    [Required]
    public string UserId { get; set; }
    [ForeignKey(nameof(GradeId))]
    public virtual Grade Grade { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser ApplicationUser { get; set; }
}