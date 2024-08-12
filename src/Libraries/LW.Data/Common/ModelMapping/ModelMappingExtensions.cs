using LW.Data.Entities;
using LW.Data.Persistence;
using LW.Infrastructure.Extensions;
using LW.Shared.DTOs.Member;

namespace LW.Data.Common.ModelMapping;

public static class ModelMappingExtensions
{
    public static MemberDto ToUserDto(this ApplicationUser applicationUser, AppDbContext context)
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
            Roles = context.UserRoles
                .Where(ur => ur.UserId == applicationUser.Id)
                .Join(context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name)
                .Select(r => r.ConvertRoleName())
                .ToList()
        };
    }
}