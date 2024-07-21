using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.ExamCode;

public class ExamCodeCreateRangeDto
{
    [Required]
    public int ExamId { get; set; }
    
    public List<CodeDto> CodeDtos { get; set; }
}