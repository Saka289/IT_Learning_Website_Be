using FluentValidation;
using LW.Shared.DTOs.User;

namespace LW.API.Application.Validators.UserValidator;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress();
        RuleFor(x => x.UserId).NotEmpty().NotNull();
        RuleFor(x => x.FirstName).NotEmpty().NotNull();
        RuleFor(x => x.LastName).NotEmpty().NotNull();
        RuleFor(x => x.PhoneNumber).NotEmpty().NotNull();
        RuleFor(x => x.Dob).NotEmpty().NotNull();
        RuleFor(x => x.Image).NotEmpty().NotNull();
    }
}