using FluentValidation;
using LW.Shared.DTOs.QuizQuestion;

namespace LW.API.Application.Validators.QuizQuestionValidator;

public class UpdateQuizQuestionCommandValidator :  AbstractValidator<QuizQuestionUpdateDto>
{
    public UpdateQuizQuestionCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Type).NotNull().NotEmpty().IsInEnum();
        RuleFor(x => x.IsActive).NotNull().NotEmpty();
        RuleFor(x => x.IsShuffle).NotNull().NotEmpty();
        RuleFor(x => x.QuestionLevel).NotNull().NotEmpty().IsInEnum();
        RuleFor(x => x.QuizId).NotNull().NotEmpty();
        RuleFor(x => x.QuizAnswers).NotNull().NotEmpty();
    }
}