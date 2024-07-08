using FluentValidation;
using LW.Shared.DTOs.CommentDocumentDto;
using LW.Shared.DTOs.Exam;

namespace LW.API.Application.Validators.ExamValidator;

public class CreateExamCommandValidator:AbstractValidator<ExamCreateDto>
{
    public CreateExamCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Province).NotNull().NotEmpty();
        RuleFor(x => x.ExamFile).Must(BeAValidPdfFormat).When(x=>x.ExamFile != null)
            .WithMessage("ExamFile must be in a valid format (pdf).");
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.Year).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.NumberQuestion).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.IsActive).NotNull();
        // Kiểm tra từng ảnh trong danh sách Images
        RuleForEach(x => x.Images)
            .Must(BeAValidImageFormat)
            .WithMessage("Each image must be in a valid format (.jpg, .jpeg, .png).");
    }
    private bool BeAValidPdfFormat(IFormFile image)
    {
        var allowedExtensions = new[] { ".pdf" };
        var extension = Path.GetExtension(image.FileName).ToLower();
        return allowedExtensions.Contains(extension);
    }
    private bool BeAValidImageFormat(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        return allowedExtensions.Contains(extension);
    }
}