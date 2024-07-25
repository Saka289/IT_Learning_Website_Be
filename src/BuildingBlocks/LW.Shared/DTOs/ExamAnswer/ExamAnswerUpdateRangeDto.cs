using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.ExamAnswer;

public class ExamAnswerUpdateRangeDto
{
    [Required] public int ExamCodeId { get; set; }
    public IEnumerable<AnswerUpdateDto> AnswerUpdateDtos { get; set; }
}