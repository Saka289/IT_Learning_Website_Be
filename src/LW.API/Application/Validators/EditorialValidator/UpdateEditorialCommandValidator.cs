using FluentValidation;
using LW.Shared.DTOs.Editorial;

namespace LW.API.Application.Validators.EditorialValidator;

public class UpdateEditorialCommandValidator :  AbstractValidator<EditorialUpdateDto>
{
    public UpdateEditorialCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.ProblemId).NotNull().NotEmpty();
    }
}