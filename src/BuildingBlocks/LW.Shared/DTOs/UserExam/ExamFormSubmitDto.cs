namespace LW.Shared.DTOs.UserExam;

public class ExamFormSubmitDto
{
    public string? UserId { get; set; }
    public int ExamCodeId { get; set; }
    public IEnumerable<UserAnswerDto> UserAnswerDtos { get; set; }
}