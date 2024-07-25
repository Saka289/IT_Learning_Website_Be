using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LW.Shared.DTOs.ExamCode;

public class ExamCodeCreateDto
{
    [Required]
    public string Code { get; set; }
    [Required]
    public IFormFile ExamFileUpload { get; set; }
    [Required]
    public int ExamId { get; set; }
}