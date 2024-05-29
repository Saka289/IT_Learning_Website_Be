using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.Admin;

public class ResetPasswordAdminDto
{
    [Required]
    public string Token { get; set; }
    [Required,EmailAddress]
    public string  Email { get; set; }
    [Required]
    public string NewPassword { get; set; }
}