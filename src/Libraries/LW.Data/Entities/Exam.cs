using System.ComponentModel.DataAnnotations;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Exam : EntityAuditBase<int>
{
    [Required]
    public string? Title { get; set; }
    [Required]
    public string? KeyWord { get; set; }
    [Required]
    public string? Province { get; set; }
    
    public string? PublicId { get; set; }
    public string? ExamFile { get; set; }
    [Required]
    public string? Description { get; set; }
    [Required]
    public int Year { set; get; }
    [Required]
    public int NumberQuestion { set; get; }
    [Required]
    public bool IsActive { get; set; }
    
    public virtual ICollection<ExamAnswer> ExamAnswers { get; set; }
    public virtual ICollection<ExamImage> ExamImages { get; set; }

    
}