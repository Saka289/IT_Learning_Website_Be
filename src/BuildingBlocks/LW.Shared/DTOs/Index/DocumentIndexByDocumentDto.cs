using LW.Shared.DTOs.Index;

namespace LW.Shared.DTOs.Index;

public class DocumentIndexByDocumentDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public IEnumerable<TopicIndexByDocumentDto> Topics { get; set; }
}