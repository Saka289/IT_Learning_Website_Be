using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.Level;

public class LevelDtoForCreate
{
    [Required]
    public string? Title { get; set; }

    [Required] public bool IsActive { get; set; } = true;
}