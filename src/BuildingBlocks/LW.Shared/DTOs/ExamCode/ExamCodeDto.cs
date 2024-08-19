namespace LW.Shared.DTOs.ExamCode;

public class ExamCodeDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string ExamFile { get; set; }
    public string PublicExamId { get; set; }
    public int ExamId { get; set; }
    public string ExamTitle { get; set; }
    public int NumberQuestion { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}