using LW.Contracts.Domains;
using Microsoft.AspNetCore.Identity;

namespace LW.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public DateOnly? Dob { get; set; }
    public string? Image { get; set; }
    
    public string? PublicId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? UserClassId  { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}