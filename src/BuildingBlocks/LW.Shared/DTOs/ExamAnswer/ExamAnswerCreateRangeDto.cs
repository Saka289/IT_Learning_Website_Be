using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.ExamAnswer;

public class ExamAnswerCreateRangeDto
{
    [Required]
    public int ExamId { get; set; }

    public IEnumerable<AnswerDto> AnswerDtos { get; set; }
}