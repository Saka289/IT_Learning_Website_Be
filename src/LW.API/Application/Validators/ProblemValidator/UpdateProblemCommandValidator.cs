using FluentValidation;
using LW.Shared.DTOs.Problem;

namespace LW.API.Application.Validators.ProblemValidator;

public class UpdateProblemCommandValidator : AbstractValidator<ProblemUpdateDto>
{
    public UpdateProblemCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.Difficulty).NotNull().NotEmpty().IsInEnum();
        RuleFor(x => x.IsActive).NotNull().NotEmpty();
        RuleFor(x => x.GradeId).NotNull().GreaterThan(0);
        RuleFor(x => x).Must(HaveValidTopicOrLesson).WithMessage("Either TopicId or LessonId should have a value, but not both.");
    }
    
    private bool HaveValidTopicOrLesson(ProblemUpdateDto problem)
    {
        return !(problem.TopicId.HasValue && problem.LessonId.HasValue) || (!problem.TopicId.HasValue && !problem.LessonId.HasValue);
    }
}