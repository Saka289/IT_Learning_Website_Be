using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class ExamCode: EntityAuditBase<int>
{
    [Required]
    public string Code { get; set; }
    [Required]
    public string ExamFile { get; set; }
    
    public string? PublicExamId { get; set; }
    [Required]
    public int ExamId { get; set; }
    [ForeignKey(nameof(ExamId))] 
    public virtual Exam? Exam { get; set; }
    
    public ICollection<ExamAnswer> ExamAnswers { get; set; }
}