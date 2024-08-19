using FluentValidation;
using LW.Shared.DTOs.QuizQuestionRelation;

namespace LW.API.Application.Validators.QuizQuestionRelationValidator;

public class UpdateQuizQuestionRelationCommandValidator : AbstractValidator<QuizQuestionRelationUpdateDto>
{
    public UpdateQuizQuestionRelationCommandValidator()
    {
        RuleFor(x => x.QuizId).NotNull().NotEmpty();
        RuleFor(x => x.QuizQuestionIds).NotNull().NotEmpty();
    }
}