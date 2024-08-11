using System.ComponentModel.DataAnnotations;
using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Grade;

public class SearchGradeDto : SearchRequestValue
{
    [Required]
    public bool IsInclude { get; set; } = false;
}