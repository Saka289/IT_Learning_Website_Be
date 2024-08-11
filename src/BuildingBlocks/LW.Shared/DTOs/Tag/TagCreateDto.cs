using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.Tag;

public class TagCreateDto
{
    [Required]
    public string? Title { get; set; }
    [Required]
    public string? KeyWord { get; set; }
    [Required]
    public bool IsActive { get; set; } = false;
}
