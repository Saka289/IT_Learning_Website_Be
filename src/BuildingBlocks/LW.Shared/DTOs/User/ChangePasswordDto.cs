using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.User;

public class ChangePasswordDto
{
    [Required, EmailAddress]
    public string Email { get; set; }
    public string? Password { get; set; }
    [Required]
    public string NewPassword { get; set; }
    public bool LoginProvider { get; set; } = false;
}