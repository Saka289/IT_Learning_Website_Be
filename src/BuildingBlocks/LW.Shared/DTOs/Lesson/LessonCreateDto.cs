using Microsoft.AspNetCore.Http;

namespace LW.Shared.DTOs.Lesson;

public class LessonCreateDto
{
    public string Title { get; set; }
    private bool IsActive { get; set; } = false;
    public string? Content { get; set; }
    public IFormFile? FilePath { get; set; }
    public int TopicId { get; set; }
    public IEnumerable<string> tagValues { get; set; }
}