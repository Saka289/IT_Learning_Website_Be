using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.ExamAnswer;

public class ExamAnswerUpdateDto
{
    [Required]
    public int Id { get; set; }
    [Required]
    public int NumberOfQuestion { get; set; }
    [Required]

    public char Answer { get; set; } //ABCD

    [Required]

    public int ExamId { get; set; }
}