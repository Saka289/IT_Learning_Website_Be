using System.ComponentModel.DataAnnotations;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Competition : EntityAuditBase<int>
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public bool IsActive { get; set; }
    
    public ICollection<Exam> Exams { get; set; }
}