using FluentValidation;
using LW.Shared.DTOs.TestCase;

namespace LW.API.Application.Validators.TestCaseValidator;

public class UpdateTestCaseCommandValidator : AbstractValidator<TestCaseUpdateDto>
{
    public UpdateTestCaseCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.OutputView).NotNull().NotEmpty();
        RuleFor(x => x.Output).NotNull().NotEmpty();
        RuleFor(x => x.IsHidden).NotNull();
        RuleFor(x => x.IsActive).NotNull();
        RuleFor(x => x.ProblemId).NotNull().NotEmpty();
    }
}