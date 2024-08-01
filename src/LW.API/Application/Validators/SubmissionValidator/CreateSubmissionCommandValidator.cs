using FluentValidation;
using LW.Shared.DTOs.Submission;

namespace LW.API.Application.Validators.SubmissionValidator;

public class CreateSubmissionCommandValidator : AbstractValidator<SubmitProblemDto>
{
    public CreateSubmissionCommandValidator()
    {
        RuleFor(x => x.ProblemId).NotNull().NotEmpty();
        RuleFor(x => x.LanguageId).NotNull().NotEmpty();
        RuleFor(x => x.SourceCode).NotNull().NotEmpty();
        RuleFor(x => x.UserId).NotNull().NotEmpty();
        RuleFor(x => x.Submit).NotNull();
    }
}