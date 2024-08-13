using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.Tag;

public class TagUpdateDto
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public bool IsActive { get; set; }
}