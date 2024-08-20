using System.ComponentModel.DataAnnotations;

namespace LW.Shared.Enums;

public enum EDifficulty
{
    [Display(Name = "Dễ")] 
    Easy = 1,
    [Display(Name = "Trung Bình")] 
    Medium = 2,
    [Display(Name = "Khó")] 
    Hard = 3,
}