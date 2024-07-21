using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;
using LW.Shared.DTOs.UserQuiz;

namespace LW.Data.Entities;

public class UserQuiz :  EntityAuditBase<int>
{
    [Required]
    [Column(TypeName = "decimal(12,2)")] 
    public decimal Score { get; set; }
    [Required]
    public int  NumberCorrect { get; set; }
    [Required]
    public int TotalQuestion { get; set; }
    [Required]
    [Column(TypeName = "json")]
    public List<HistoryQuiz>? HistoryQuizzes { get; set; }
    [Required]
    public string UserId { get; set; }
    [Required]
    public int QuizId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser ApplicationUser { get; set; }
    [ForeignKey(nameof(QuizId))]
    public virtual Quiz Quiz { get; set; }
}