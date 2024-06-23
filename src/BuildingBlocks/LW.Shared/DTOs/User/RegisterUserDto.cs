using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.User;

public class RegisterUserDto
{
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string FirstName{ get; set; }
    [Required]
    public string LastName{ get; set; }
    [Phone]
    public string? PhoneNumber { get; set; }
    [Required]
    public string Password { get; set; }
}