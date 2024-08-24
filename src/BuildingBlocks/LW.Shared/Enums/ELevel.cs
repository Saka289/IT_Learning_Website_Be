using System.ComponentModel.DataAnnotations;

namespace LW.Shared.Enums;

public enum ELevel
{
    [Display(Name = "Trung học phổ thông")] 
    SGK = 1,
    [Display(Name = "Trung học cơ sở")] 
    SBT = 2,
    [Display(Name = "Tiểu học")] 
    SGV = 3,
    
}