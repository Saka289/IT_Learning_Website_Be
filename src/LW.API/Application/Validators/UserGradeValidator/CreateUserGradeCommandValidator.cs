using FluentValidation;
using LW.Shared.DTOs;

namespace LW.API.Application.Validators.UserGradeValidator;

public class CreateUserGradeCommandValidator: AbstractValidator<UserGradeCreateDto>
{
    public CreateUserGradeCommandValidator()
    {
        RuleFor(x => x.GradeId).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.UserId).NotNull().NotEmpty();
    }
}