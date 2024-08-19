using FluentValidation;
using LW.Shared.DTOs.Lesson;

namespace LW.API.Application.Validators.LessonValidator;

public class UpdateLessonCommandValidator : AbstractValidator<LessonUpdateDto>
{
    public UpdateLessonCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Title).NotNull().NotEmpty().Length(5, 200);
        RuleFor(x => x.TopicId).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x).Must(HaveValidContentOrFilePath).WithMessage("Either Content or FilePath must be provided, but not both.");
    }
    
    private bool HaveValidContentOrFilePath(LessonUpdateDto lesson)
    {
        // Kiểm tra chỉ một trong hai Content hoặc FilePath có giá trị và ít nhất một trong hai phải có giá trị
        return (lesson.Content != null && lesson.FilePath == null) ||
               (lesson.Content == null && lesson.FilePath != null);
    }
}