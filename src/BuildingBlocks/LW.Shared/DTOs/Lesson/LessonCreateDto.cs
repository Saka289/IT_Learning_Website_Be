using Microsoft.AspNetCore.Http;

namespace LW.Shared.DTOs.Lesson;

public class LessonCreateDto
{
    public string Title { get; set; }
    public bool IsActive { get; set; }
    public string? Content { get; set; }
    public IFormFile? FilePath { get; set; }
    public int TopicId { get; set; }
}