using System.ComponentModel.DataAnnotations;

namespace LW.Shared.DTOs.Admin;

public class RegisterMemberDto
{
    public string FistName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; } 
    public string Password { get; set; }
    public string ConfirmPassword { get; set; } 
    public string RoleString { get; set; }
}