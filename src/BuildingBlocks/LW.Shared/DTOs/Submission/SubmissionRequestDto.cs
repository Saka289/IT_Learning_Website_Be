using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.Submission;

public class SubmissionRequestDto
{
    [Required]
    public int ProblemId { get; set; }
    [Required]
    public string UserId { get; set; }
    public int? LanguageId { get; set; } = 0;
}