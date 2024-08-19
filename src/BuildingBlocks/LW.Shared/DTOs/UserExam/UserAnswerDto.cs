using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.UserExam;

public class UserAnswerDto
{
    public int NumberOfQuestion { get; set; }  //1 2 3
    public string? Answer { get; set; } // A B C D
}