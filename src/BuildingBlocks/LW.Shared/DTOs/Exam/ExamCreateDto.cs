using System.ComponentModel.DataAnnotations;
using LW.Shared.DTOs.Tag;
using LW.Shared.Enums;
using Microsoft.AspNetCore.Http;

namespace LW.Shared.DTOs.Exam;

public class ExamCreateDto
{
    [Required]
    public int CompetitionId { get; set; }
    [Required]
    public EExamType Type { get; set; } // TL-1 TN-2
    [Required]
    public string? Title { get; set; }
    [Required]
    public string? Province { get; set; }
    
    public IFormFile? ExamEssayFileUpload { get; set; } //pdf
    public IFormFile? ExamSolutionFileUpload { get; set; } //pdf
    
    [Required]
    public string? Description { get; set; }
    [Required]
    public int Year { set; get; }
    public int NumberQuestion { set; get; }
    
    [Required]
    public IEnumerable<string>? TagValues { get; set; }
    
    [Required] public bool IsActive { get; set; } = false;
    public int? GradeId { get; set; }
}