using FluentValidation;
using LW.Shared.DTOs.Lesson;

namespace LW.API.Application.Validators.LessonValidator;

public class UpdateLessonCommandValidator : AbstractValidator<LessonUpdateDto>
{
    public UpdateLessonCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.TopicId).NotNull().NotEmpty().GreaterThan(0);
    }
}