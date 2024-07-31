using FluentValidation;
using LW.Shared.Solution;

namespace LW.API.Application.Validators.SolutionValidator;

public class UpdateSolutionCommandValidator : AbstractValidator<SolutionUpdateDto>
{
    public UpdateSolutionCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.Coding).NotNull().NotEmpty();
        RuleFor(x => x.ProblemId).NotNull().NotEmpty();
        RuleFor(x => x.UserId).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotNull().NotEmpty();
    }
}