using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.ExamAnswer;

public class ExamAnswerCreateRangeDto
{
    [Required]
    public int ExamCodeId { get; set; }
    public IEnumerable<AnswerDto> AnswerDtos { get; set; }
}