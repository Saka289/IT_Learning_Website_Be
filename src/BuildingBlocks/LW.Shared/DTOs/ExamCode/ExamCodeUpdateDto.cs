using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LW.Shared.DTOs.ExamCode;

public class ExamCodeUpdateDto
{
    [Required]
    public int Id { get; set; }
    
    public string? Code { get; set; }
    
    public IFormFile? ExamFileUpload { get; set; }
    
    public int ExamId { get; set; }
}