using FluentValidation;
using LW.Shared.DTOs.ProgramLanguage;

namespace LW.API.Application.Validators.ProgramLanguageValidator;

public class CreateProgramLanguageCommandValidator : AbstractValidator<ProgramLanguageCreateDto>
{
    public CreateProgramLanguageCommandValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.BaseId).NotNull().GreaterThan(0);
        RuleFor(x => x.IsActive).NotNull().NotEmpty();
    }
}