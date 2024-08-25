using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Document;

public class SearchDocumentDto : SearchRequestValue
{
    public int? GradeId { get; set; }
    public bool? Status { get; set; }
}