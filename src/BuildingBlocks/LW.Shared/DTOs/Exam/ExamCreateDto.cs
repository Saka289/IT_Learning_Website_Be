using System.ComponentModel.DataAnnotations;
using LW.Shared.DTOs.Tag;
using Microsoft.AspNetCore.Http;

namespace LW.Shared.DTOs.Exam;

public class ExamCreateDto
{
    [Required]
    public string? Title { get; set; }
    [Required]
    public string? Province { get; set; }
    
    public IFormFile? ExamFile { get; set; } //pdf
    [Required]
    public string? Description { get; set; }
    [Required]
    public int Year { set; get; }
    [Required]
    public int NumberQuestion { set; get; }
    
    [Required]
    public IEnumerable<string> tagValues { get; set; }
    
    [Required] public bool IsActive { get; set; } = false;
    
    
    
    public List<IFormFile>? Images { get; set; } 
    
}