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
        RuleFor(x => x).Must(HaveValidTopicLessonGrade).WithMessage("Only one of TopicId, LessonId, or GradeId should have a value, the others must be null.");
    }
    
    private bool HaveValidTopicLessonGrade(ProblemUpdateDto problem)
    {
        int count = 0;

        if (problem.TopicId.HasValue) count++;
        if (problem.LessonId.HasValue) count++;
        if (problem.GradeId.HasValue) count++;
        
        return count == 1;
    }
}