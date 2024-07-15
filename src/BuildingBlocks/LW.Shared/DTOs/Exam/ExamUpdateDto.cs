using Microsoft.AspNetCore.Http;

namespace LW.Shared.DTOs.Exam;

public class ExamUpdateDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Province { get; set; }
    
    public IFormFile? FileUpload { get; set; }
    public string? Description { get; set; }
    public int Year { set; get; }
    public int NumberQuestion { set; get; }
    public IEnumerable<string>? tagValues { get; set; }
    public bool IsActive { get; set; } = false;
}