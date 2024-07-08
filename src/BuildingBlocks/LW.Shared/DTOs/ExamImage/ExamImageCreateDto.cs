using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LW.Shared.DTOs;

public class ExamImageCreateDto
{
    [Required]
    public IFormFile? ImageUpload { get; set; }
 
    [Required]
    public int ExamId { get; set; }
}