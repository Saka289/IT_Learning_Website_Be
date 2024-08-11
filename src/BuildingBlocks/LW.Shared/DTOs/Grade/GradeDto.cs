using LW.Shared.DTOs.Document;

namespace LW.Shared.DTOs.Grade;

public class GradeDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string KeyWord { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
    public IEnumerable<GradeDocumentDto> Documents { get; set; }
    public IEnumerable<GradeExamDto> Exams { get; set; }
}