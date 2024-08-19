using FluentValidation;
using LW.Shared.DTOs.Quiz;

namespace LW.API.Application.Validators.QuizValidator;

public class UpdateQuizCommandValidator : AbstractValidator<QuizUpdateDto>
{
    public UpdateQuizCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
        RuleFor(x => x.Title).NotNull().NotEmpty().Length(5, 250);
        RuleFor(x => x.Description).NotNull().NotEmpty().Length(5, 250);
        RuleFor(x => x.Score).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Type).NotNull().NotEmpty().IsInEnum();
        RuleFor(x => x.IsActive).NotNull();
        RuleFor(x => x).Must(HaveValidTopicLessonGrade).WithMessage("Only one of TopicId, LessonId, or GradeId should have a value, the others must be null.");
    }
    
    private bool HaveValidTopicLessonGrade(QuizUpdateDto quiz)
    {
        int count = 0;

        if (quiz.TopicId.HasValue) count++;
        if (quiz.LessonId.HasValue) count++;
        if (quiz.GradeId.HasValue) count++;
        
        return count == 1;
    }
}