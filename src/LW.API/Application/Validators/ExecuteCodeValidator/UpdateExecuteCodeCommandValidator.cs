using FluentValidation;
using LW.Shared.DTOs.ExecuteCode;

namespace LW.API.Application.Validators.ExecuteCodeValidator;

public class UpdateExecuteCodeCommandValidator : AbstractValidator<ExecuteCodeUpdateDto>
{
    public UpdateExecuteCodeCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.SampleCode).NotNull().NotEmpty();
        RuleFor(x => x.ProblemId).NotNull().NotEmpty();
        RuleFor(x => x.LanguageId).NotNull().NotEmpty();
    }
}