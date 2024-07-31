using FluentValidation;
using LW.Shared.Solution;

namespace LW.API.Application.Validators.SolutionValidator;

public class CreateSolutionCommandValidator : AbstractValidator<SolutionCreateDto>
{
    public CreateSolutionCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.Coding).NotNull().NotEmpty();
        RuleFor(x => x.ProblemId).NotNull().NotEmpty();
        RuleFor(x => x.UserId).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotNull().NotEmpty();
    }
}