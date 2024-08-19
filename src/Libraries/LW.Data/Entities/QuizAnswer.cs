using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class QuizAnswer : EntityAuditBase<int>
{
    [Required]
    public string Content { get; set; }
    [Required]
    public bool IsCorrect { get; set; }
    [Required]
    public int QuizQuestionId { get; set; }
    [ForeignKey(nameof(QuizQuestionId))]
    public virtual QuizQuestion QuizQuestion { get; set; }
}