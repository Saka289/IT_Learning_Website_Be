using FluentValidation;
using LW.Shared.DTOs.Quiz;

namespace LW.API.Application.Validators.QuizValidator;

public class CreateQuizCommandValidator : AbstractValidator<QuizCreateDto>
{
    public CreateQuizCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty().Length(5, 250);
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.Score).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Type).NotNull().NotEmpty().IsInEnum();
        RuleFor(x => x.IsActive).NotNull().NotEmpty();
        RuleFor(x => x).Must(HaveValidTopicOrLesson).WithMessage("Either TopicId or LessonId should have a value, but not both.");
    }

    private bool HaveValidTopicOrLesson(QuizCreateDto quiz)
    {
        return !(quiz.TopicId.HasValue && quiz.LessonId.HasValue) || (!quiz.TopicId.HasValue && !quiz.LessonId.HasValue);
    }
}