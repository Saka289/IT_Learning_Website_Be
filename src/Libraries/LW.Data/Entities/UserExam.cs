using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class UserExam : EntityAuditBase<int>
{
    [Required]
    public decimal Score { get; set; }
    [Required]
    [Column(TypeName = "json")]
     public string HistoryExam { get; set; }
    [Required]
    public string? UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser? ApplicationUser { get; set; }
    [Required]
    public int ExamId { get; set; }
    [ForeignKey(nameof(ExamId))]
    public virtual Exam? Exam { get; set; }
}