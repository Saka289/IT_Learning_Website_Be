using FluentValidation;
using LW.Shared.DTOs.Admin;

namespace LW.API.Application.Validators.AdminValidator;

public class RegisterMemberCommandValidator:AbstractValidator<RegisterMemberDto>
{
    public RegisterMemberCommandValidator()
    {
        RuleFor(x => x.FistName).NotEmpty().NotNull();
        RuleFor(x => x.LastName).NotEmpty().NotNull();
        RuleFor(x => x.Username).NotEmpty().NotNull();
        RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().NotNull();
        RuleFor(x => x.ConfirmPassword).NotEmpty().NotNull();
        RuleFor(x => x.RoleString).NotEmpty().NotNull();
    }
}