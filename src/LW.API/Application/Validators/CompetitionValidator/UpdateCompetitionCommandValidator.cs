using FluentValidation;
using LW.Shared.DTOs.Competition;

namespace LW.API.Application.Validators.CompetitionValidator;

public class UpdateCompetitionCommandValidator:AbstractValidator<CompetitionUpdateDto>
{
    public UpdateCompetitionCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotNull();
    }
}