using LW.Data.Entities;
using LW.Data.Persistence;
using LW.Shared.Constant;
using LW.Shared.Enums;
using Microsoft.OpenApi.Extensions;

using Microsoft.AspNetCore.Identity;

namespace LW.Services.UserService;

public static class CreateUserFromSocialLoginExtension
{
    /// <summary>
    /// Creates user from social login
    /// </summary>
    /// <param name="userManager">the usermanager</param>
    /// <param name="model">the model</param>
    /// <param name="loginProvider">the login provider</param>
    /// <returns>System.Threading.Tasks.Task&lt;User&gt;</returns>
    /// 
    public static async Task<ApplicationUser> CreateUserFromSocialLogin(this UserManager<ApplicationUser> userManager, CreateUserFromSocialLogin model, ELoginProvider loginProvider)
    {
        var user = await userManager.FindByLoginAsync(loginProvider.GetDisplayName(), model.LoginProviderSubject);

        if (user is not null)
        {
            return user; 
        }

        user = await userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            user = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                Image = model.ProfilePicture
            };
            user.EmailConfirmed = true;
            
            await userManager.CreateAsync(user);
            await userManager.AddToRoleAsync(user, RoleConstant.RoleUser);
        }

        UserLoginInfo userLoginInfo = null;
        switch (loginProvider)
        {
            case ELoginProvider.Google:
            {
                userLoginInfo = new UserLoginInfo(loginProvider.GetDisplayName(), model.LoginProviderSubject, loginProvider.GetDisplayName().ToUpper());
            }
                break;
            case ELoginProvider.Facebook:
            {
                userLoginInfo = new UserLoginInfo(loginProvider.GetDisplayName(), model.LoginProviderSubject, loginProvider.GetDisplayName().ToUpper());
            }
                break;
            default:
                break;
        }
        
        var result = await userManager.AddLoginAsync(user, userLoginInfo);

        if (!result.Succeeded)
        {
            return null;
        }

        return user;
    }
}