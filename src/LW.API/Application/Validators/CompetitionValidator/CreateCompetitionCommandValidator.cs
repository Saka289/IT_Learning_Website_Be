using System.Data;
using FluentValidation;
using LW.Shared.DTOs.Competition;

namespace LW.API.Application.Validators.CompetitionValidator;

public class CreateCompetitionCommandValidator : AbstractValidator<CompetitionCreateDto>
{
    public CreateCompetitionCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotNull();
    }
}