using FluentValidation;
using LW.Shared.DTOs.Quiz;

namespace LW.API.Application.Validators.QuizValidator;

public class CreateQuizCommandValidator : AbstractValidator<QuizCreateDto>
{
    public CreateQuizCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.Score).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Type).NotNull().NotEmpty().IsInEnum();
        RuleFor(x => x.IsActive).NotNull().NotEmpty();
        RuleFor(x => x.tagValues).NotNull().NotEmpty();
        RuleFor(x => x).Must(HaveValidTopicOrLesson).WithMessage("If TopicId is set, LessonId must be null, and vice versa.");
    }
    
    private bool HaveValidTopicOrLesson(QuizCreateDto quiz)
    {
        return !(quiz.TopicId.HasValue && quiz.LessonId.HasValue);
    }
}