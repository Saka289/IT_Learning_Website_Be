using System.ComponentModel.DataAnnotations;

namespace LW.Shared.Enums;

public enum EStatusProblem
{
    [Display(Name = "Chưa làm")]
    ToDo = 1,
    [Display(Name = "Đã làm")]
    Solved = 2,
}