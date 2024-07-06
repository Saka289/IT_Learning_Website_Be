using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;
using LW.Shared.DTOs.Quiz;
using LW.Shared.Enums;
using MimeKit.Encodings;

namespace LW.Data.Entities;

public class QuizQuestion : EntityAuditBase<int>
{
    [Required]
    public ETypeQuestion Type { get; set; }
    [Required]
    public string Content { get; set; }
    [Required]
    public bool IsActive { get; set; }
    [Required]
    public EQuestionLevel QuestionLevel { get; set; }
    [Required]
    public int QuizId { get; set; }
    [ForeignKey(nameof(QuizId))]
    public virtual Quiz Quiz { get; set; }
    public virtual ICollection<QuizAnswer> QuizAnswers { get; set; }
}