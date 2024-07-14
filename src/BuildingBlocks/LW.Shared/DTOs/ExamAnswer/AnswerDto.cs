using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.ExamAnswer;

public class AnswerDto
{
    [Required]
    public int NumberOfQuestion { get; set; }
    
    [Required]

    public char Answer { get; set; } //ABCD
}