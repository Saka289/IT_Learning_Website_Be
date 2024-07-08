using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LW.Shared.DTOs;

public class ExamImageUpdateDto
{
    [Required]
    public int Id { get; set; }
    
    public IFormFile? ImageUpload { get; set; }
    
    [Required]
    public int Index { get; set; }
    [Required]
    public int ExamId { get; set; }
}