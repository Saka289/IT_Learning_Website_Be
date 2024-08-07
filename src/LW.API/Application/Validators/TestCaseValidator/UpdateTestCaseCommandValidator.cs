using FluentValidation;
using LW.Shared.DTOs.TestCase;

namespace LW.API.Application.Validators.TestCaseValidator;

public class UpdateTestCaseCommandValidator : AbstractValidator<TestCaseUpdateDto>
{
    public UpdateTestCaseCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Output).NotNull().NotEmpty();
        RuleFor(x => x.IsHidden).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotNull().NotEmpty();
        RuleFor(x => x.ProblemId).NotNull().NotEmpty();
    }
}