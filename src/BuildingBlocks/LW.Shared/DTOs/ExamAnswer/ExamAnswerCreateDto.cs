using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.ExamAnswer;

public class ExamAnswerCreateDto
{
    [Required]
    public int NumberOfQuestion { get; set; }
    [Required]

    public char Answer { get; set; } //ABCD
    [Required]

    public decimal Score { get; set; }
    [Required]

    public int ExamId { get; set; }
}