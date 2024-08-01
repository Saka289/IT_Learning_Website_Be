using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.UserExam;

public class UserAnswerDto
{
    [Required]
    public int NumberOfQuestion { get; set; }  //1 2 3
    [Required]
    public char Answer { get; set; } // A B C D
}