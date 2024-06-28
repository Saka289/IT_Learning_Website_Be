using FluentValidation;
using LW.Shared.DTOs.Lesson;

namespace LW.API.Application.Validators.LessonValidator;

public class CreateLessonCommandValidator : AbstractValidator<LessonCreateDto>
{
    public CreateLessonCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotEmpty();
        RuleFor(x => x.TopicId).NotNull().NotEmpty().GreaterThan(0);
    }
}