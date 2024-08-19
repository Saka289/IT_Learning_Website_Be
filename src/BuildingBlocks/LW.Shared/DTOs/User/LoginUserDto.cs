using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.User;

public class LoginUserDto
{
    [Required]
    public string EmailOrUserName { get; set; }
    [Required]
    public string Password { get; set; }
}