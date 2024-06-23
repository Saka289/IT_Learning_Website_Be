using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.Level;

public class LevelDtoForUpdate
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string? Title { get; set; }
    [Required]
    public bool IsActive { get; set; }
}