using FluentValidation;
using LW.Shared.DTOs.TestCase;

namespace LW.API.Application.Validators.TestCaseValidator;

public class CreateTestCaseCommandValidator : AbstractValidator<TestCaseCreateDto>
{
    public CreateTestCaseCommandValidator()
    {
        RuleFor(x => x.Input).NotNull().NotEmpty();
        RuleFor(x => x.Output).NotNull().NotEmpty();
        RuleFor(x => x.IsHidden).NotNull();
        RuleFor(x => x.IsActive).NotNull();
        RuleFor(x => x.ProblemId).NotNull().NotEmpty();
    }
}