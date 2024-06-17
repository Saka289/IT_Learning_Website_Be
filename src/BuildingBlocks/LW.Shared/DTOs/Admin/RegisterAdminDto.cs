using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.Admin;

public class RegisterAdminDto
{
    [Required]
    public string FistName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Username { get; set; }
    [Required, EmailAddress] 
    public string Email { get; set; } 
    [Required]
    public string Password { get; set; }
    [Required]
    public string ConfirmPassword { get; set; } 
    [Required]
    public string RoleString { get; set; }
}