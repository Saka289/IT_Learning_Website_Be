using FluentValidation;
using LW.Shared.DTOs.QuizQuestion;

namespace LW.API.Application.Validators.QuizQuestionValidator;

public class CreateQuizQuestionCommandValidator : AbstractValidator<QuizQuestionCreateDto>
{
    public CreateQuizQuestionCommandValidator()
    {
        RuleFor(x => x.Type).NotNull().NotEmpty().IsInEnum();
        RuleFor(x => x.IsActive).NotNull().NotEmpty();
        RuleFor(x => x.QuestionLevel).NotNull().NotEmpty().IsInEnum();
        RuleFor(x => x.IsShuffle).NotNull().NotEmpty();
        RuleFor(x => x.QuizAnswers).NotNull().NotEmpty();
    }
}