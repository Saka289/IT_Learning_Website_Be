using FluentValidation;
using LW.Shared.DTOs.ExecuteCode;

namespace LW.API.Application.Validators.ExecuteCodeValidator;

public class CreateExecuteCodeCommandValidator : AbstractValidator<ExecuteCodeCreateDto>
{
    public CreateExecuteCodeCommandValidator()
    {
        RuleFor(x => x.MainCode).NotNull().NotEmpty();
        RuleFor(x => x.SampleCode).NotNull().NotEmpty();
        RuleFor(x => x.ProblemId).NotNull().NotEmpty();
        RuleFor(x => x.LanguageId).NotNull().NotEmpty();
    }
}