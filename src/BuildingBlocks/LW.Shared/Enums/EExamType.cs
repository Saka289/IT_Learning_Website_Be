using System.ComponentModel.DataAnnotations;

namespace LW.Shared.Enums;

public enum EExamType
{
    [Display(Name = "Tự luận")] 
    TL = 1,
    [Display(Name = "Trắc nghiệm")] 
    TN = 2,
}