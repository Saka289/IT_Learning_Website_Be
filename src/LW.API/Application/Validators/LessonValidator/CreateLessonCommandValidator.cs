using FluentValidation;
using LW.Shared.DTOs.Lesson;

namespace LW.API.Application.Validators.LessonValidator;

public class CreateLessonCommandValidator : AbstractValidator<LessonCreateDto>
{
    public CreateLessonCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty().Length(5, 250);
        RuleFor(x => x.TopicId).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Index).NotNull().GreaterThan(0);
        RuleFor(x => x).Must(HaveValidContentOrFilePath)
            .WithMessage("Either Content or FilePath must be provided, but not both.");
    }

    private bool HaveValidContentOrFilePath(LessonCreateDto lesson)
    {
        // Kiểm tra chỉ một trong hai Content hoặc FilePath có giá trị và ít nhất một trong hai phải có giá trị
        return (lesson.Content != null && lesson.FilePath == null) ||
               (lesson.Content == null && lesson.FilePath != null);
    }
}