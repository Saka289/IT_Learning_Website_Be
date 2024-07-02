using FluentValidation;
using LW.Shared.DTOs;

namespace LW.API.Application.Validators.UserGradeValidator;

public class UpdateUserGradeCommandValidator: AbstractValidator<UserGradeUpdateDto>
{
    public UpdateUserGradeCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.GradeId).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.UserId).NotNull().NotEmpty();
    }
}