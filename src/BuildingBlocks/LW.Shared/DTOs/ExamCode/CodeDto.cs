using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LW.Shared.DTOs.ExamCode;

public class CodeDto
{
    [Required]
    public string Code { get; set; }
    [Required]
    public IFormFile ExamFileUpload { get; set; }
}