using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Document;

public class SearchDocumentDto : SearchRequestParameters
{
    public override string? Key { get; set; } = "keyWord";
}