using FluentValidation;
using LW.Shared.DTOs.ProgramLanguage;

namespace LW.API.Application.Validators.ProgramLanguageValidator;

public class UpdateProgramLanguageCommandValidator : AbstractValidator<ProgramLanguageUpdateDto>
{
    public UpdateProgramLanguageCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().GreaterThan(0);
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.BaseId).NotNull().GreaterThan(0);
        RuleFor(x => x.IsActive).NotNull();
    }
}