using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.Admin;

public class LoginAdminDto
{
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}