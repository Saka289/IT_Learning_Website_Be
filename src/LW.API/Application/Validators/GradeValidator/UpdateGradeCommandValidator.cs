using FluentValidation;
using LW.Shared.DTOs.Grade;

namespace LW.API.Application.Validators.GradeValidator;

public class UpdateGradeCommandValidator: AbstractValidator<GradeUpdateDto>
{
    public UpdateGradeCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotEmpty();
    }
}