using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;
using LW.Shared.Enums;

namespace LW.Data.Entities;

public class Exam : EntityAuditBase<int>
{
    [Required] 
    public EExamType Type { get; set; }
    [Required]
    public string? Title { get; set; }
    [Required]
    public string? KeyWord { get; set; }
    [Required]
    public string? Province { get; set; }
    
    public string? PublicExamEssayId { get; set; }
    
    public string? PublicExamEssaySolutionId { get; set; }
    public string? ExamEssayFile { get; set; }
    
    public string? ExamSolutionFile { get; set; }
    
    public string? UrlDownloadSolutionFile { get; set; }

    [Required]
    public string? Description { get; set; }
    [Required]
    public int Year { set; get; }
    [Required]
    public int NumberQuestion { set; get; }
    [Required]
    public bool IsActive { get; set; }
    [Required]
    public int CompetitionId { get; set; }
    [ForeignKey(nameof(CompetitionId))]
    public virtual Competition Competition { get; set; }
    public int? GradeId { get; set; }
    [ForeignKey(nameof(GradeId))]
    public virtual Grade? Grade { get; set; }
    public virtual ICollection<ExamCode>? ExamCodes { get; set; }
}