using FluentValidation;
using LW.Shared.DTOs.Quiz;

namespace LW.API.Application.Validators.QuizValidator;

public class UpdateQuizCommandValidator : AbstractValidator<QuizUpdateDto>
{
    public UpdateQuizCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.Score).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Type).NotNull().NotEmpty().IsInEnum();
        RuleFor(x => x.IsActive).NotNull().NotEmpty();
    }
}