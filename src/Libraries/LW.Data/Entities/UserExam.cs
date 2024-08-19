using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class UserExam : EntityAuditBase<int>
{
    [Required]
    [Column(TypeName = "decimal(12,2)")] 
    public decimal Score { get; set; }
    [Required]
    [Column(TypeName = "json")]
    public List<HistoryAnswer> HistoryExam { get; set; }
    [Required]
    public string? UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser? ApplicationUser { get; set; }
    [Required]
    public int ExamCodeId { get; set; }
    [ForeignKey(nameof(ExamCodeId))]
    public virtual ExamCode? ExamCode { get; set; }
}