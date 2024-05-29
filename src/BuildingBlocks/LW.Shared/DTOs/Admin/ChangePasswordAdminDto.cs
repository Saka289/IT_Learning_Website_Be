using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.Admin;

public class ChangePasswordAdminDto
{
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string NewPassword { get; set; }
}