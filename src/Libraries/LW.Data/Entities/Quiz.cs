using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;
using LW.Shared.Enums;

namespace LW.Data.Entities;

public class Quiz : EntityAuditBase<int>
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string KeyWord { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public ETypeQuiz Type { get; set; }
    [Required]
    [Column(TypeName = "decimal(12,2)")] 
    public decimal Score { get; set; }
    [Required]
    public bool IsShuffle { get; set; }
    [Required]
    public bool IsActive { get; set; }
    public int? TopicId { get; set; }
    public int? LessonId { get; set; }
    [ForeignKey(nameof(TopicId))]
    public virtual Topic Topic { get; set; }
    [ForeignKey(nameof(LessonId))]
    public virtual Lesson Lesson { get; set; }
    public virtual ICollection<QuizQuestionRelation> QuizQuestionRelations { get; set; }
    public virtual ICollection<UserQuiz> UserQuizzes { get; set; }
}