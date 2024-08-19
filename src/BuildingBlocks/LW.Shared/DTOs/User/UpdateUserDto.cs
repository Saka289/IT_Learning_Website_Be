using Microsoft.AspNetCore.Http;

namespace LW.Shared.DTOs.User;

public class UpdateUserDto
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime Dob { get; set; }
    public IFormFile? Image { get; set; }
}