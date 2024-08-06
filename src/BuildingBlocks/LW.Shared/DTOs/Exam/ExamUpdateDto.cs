using LW.Shared.Enums;
using Microsoft.AspNetCore.Http;

namespace LW.Shared.DTOs.Exam;

public class ExamUpdateDto
{
    public int CompetitionId { get; set; }
    public int Id { get; set; }
    
    public EExamType Type { get; set; } // TL-1 TN-2
    public string? Title { get; set; }
    public string? Province { get; set; }
    
    public IFormFile? ExamEssayFileUpload { get; set; }
    public IFormFile? ExamSolutionFileUpload { get; set; }
    public string? Description { get; set; }
    public int Year { set; get; }
    public int NumberQuestion { set; get; }
    public IEnumerable<string>? tagValues { get; set; }
    public bool IsActive { get; set; } = false;
    
    public int? GradeId { get; set; }

}