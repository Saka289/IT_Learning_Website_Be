using LW.Data.Entities;
using LW.Shared.DTOs.Member;
using Microsoft.AspNetCore.Identity;

namespace LW.Services.Common.ModelMapping;

public static class ModelMappingExtensions
{
    public static MemberDto ToMemberDto(this ApplicationUser applicationUser, UserManager<ApplicationUser> userManager)
    {
        return new MemberDto
        {
            Id = applicationUser.Id,
            Email = applicationUser.Email,
            UserName = applicationUser.UserName,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            PhoneNumber = applicationUser.PhoneNumber,
            Dob = Convert.ToString(applicationUser.Dob),
            Image = applicationUser.Image,
            EmailConfirmed = applicationUser.EmailConfirmed,
            LockOutEnd = applicationUser.LockoutEnd,
            Roles =  userManager.GetRolesAsync(applicationUser).Result.ToList()
        };
    }
}