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
        RuleFor(x => x).Must(HaveValidTopicOrLesson).WithMessage("If TopicId is set, LessonId must be null, and vice versa.");
    }
    
    private bool HaveValidTopicOrLesson(QuizUpdateDto quiz)
    {
        return !(quiz.TopicId.HasValue && quiz.LessonId.HasValue) && (quiz.TopicId.HasValue || quiz.LessonId.HasValue);
    }
}