using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Quiz;

namespace LW.Shared.DTOs.Grade;

public class GradeDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string KeyWord { get; set; }
    public bool IsActive { get; set; }
    public int LevelId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
    public IEnumerable<GradeDocumentDto> Documents { get; set; }
    public IEnumerable<GradeExamDto> Exams { get; set; }
    public IEnumerable<GradeProblemCustomDto> ProblemsCustom { get; set; }
    public IEnumerable<GradeQuizCustomDto> QuizzesCustom { get; set; }
}