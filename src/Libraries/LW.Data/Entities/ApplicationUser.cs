using LW.Contracts.Domains;
using Microsoft.AspNetCore.Identity;

namespace LW.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public DateOnly? Dob { get; set; }

    public string? Image { get; set; }

    public string FirstName { get; set; }
    
    public string LastName { get; set; }
}