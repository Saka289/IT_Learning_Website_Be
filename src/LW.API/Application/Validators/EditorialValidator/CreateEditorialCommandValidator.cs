using FluentValidation;
using LW.Shared.DTOs.Editorial;

namespace LW.API.Application.Validators.EditorialValidator;

public class CreateEditorialCommandValidator :  AbstractValidator<EditorialCreateDto>
{
    public CreateEditorialCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.ProblemId).NotNull().NotEmpty();
    }
}