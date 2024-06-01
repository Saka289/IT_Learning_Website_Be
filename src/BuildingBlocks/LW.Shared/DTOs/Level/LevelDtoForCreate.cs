using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.Level;

public class LevelDtoForCreate
{
    [Required]
    public string? Name { get; set; }

    [Required] public bool Active { get; set; } = true;
}