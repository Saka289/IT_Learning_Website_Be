using FluentValidation;
using LW.Shared.DTOs.QuizQuestionRelation;

namespace LW.API.Application.Validators.QuizQuestionRelationValidator;

public class CreateQuizQuestionRelationCustomCreateCommandValidator : AbstractValidator<QuizQuestionRelationCustomCreateDto>
{
    public CreateQuizQuestionRelationCustomCreateCommandValidator()
    {
        RuleFor(x => x.QuizId).NotNull().NotEmpty();
        RuleFor(x => x.QuiQuestionRelationCustomCreate).NotNull().NotEmpty();
    }
}