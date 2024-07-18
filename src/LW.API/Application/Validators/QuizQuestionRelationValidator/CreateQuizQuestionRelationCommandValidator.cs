using FluentValidation;
using LW.Shared.DTOs.QuizQuestionRelation;

namespace LW.API.Application.Validators.QuizQuestionRelationValidator;
 
public class CreateQuizQuestionRelationCommandValidator : AbstractValidator<QuizQuestionRelationCreateDto>
{
    public CreateQuizQuestionRelationCommandValidator()
    {
        RuleFor(x => x.QuizId).NotNull().NotEmpty();
        RuleFor(x => x.QuizQuestionIds).NotNull().NotEmpty();
    }
}