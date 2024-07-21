using FluentValidation;
using LW.Shared.DTOs.UserQuiz;

namespace LW.API.Application.Validators.UserQuizValidator;

public class SubmitUserQuizCommandValidator : AbstractValidator<UserQuizSubmitDto>
{
    public SubmitUserQuizCommandValidator()
    {
        RuleFor(x => x.QuestionAnswerDto).NotNull().NotEmpty();
        RuleFor(x => x.QuizId).NotNull().NotEmpty();
        RuleFor(x => x.UserId).NotNull().NotEmpty();
    }
}