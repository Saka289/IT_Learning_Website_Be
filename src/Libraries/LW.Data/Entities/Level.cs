using System.ComponentModel.DataAnnotations;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Level : EntityAuditBase<int>
{
    [Required]
    public string Name { get; set; }
    [Required]
    public bool Active { get; set; }
    
    public ICollection<Grade> Grades { get; set; }

}