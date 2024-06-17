using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.User;

public class LoginUserDto
{
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}