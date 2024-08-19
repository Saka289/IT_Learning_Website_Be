using FluentValidation;
using LW.Shared.DTOs.User;

namespace LW.API.Application.Validators.UserValidator;

public class CreateUserCommandValidator : AbstractValidator<RegisterUserDto>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress();
        RuleFor(x => x.UserName).NotEmpty().NotNull();
        RuleFor(x => x.FirstName).NotEmpty().NotNull();
        RuleFor(x => x.LastName).NotEmpty().NotNull();
        RuleFor(x => x.PhoneNumber).NotEmpty().NotNull();
        RuleFor(x => x.Password).NotEmpty().NotNull();
    }
}