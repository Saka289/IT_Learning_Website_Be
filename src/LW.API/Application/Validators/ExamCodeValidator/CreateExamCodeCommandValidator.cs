using FluentValidation;
using LW.Shared.DTOs.ExamCode;

namespace LW.API.Application.Validators.ExamCodeValidator;

public class CreateExamCodeCommandValidator : AbstractValidator<ExamCodeCreateDto>
{
    public CreateExamCodeCommandValidator()
    {
        RuleFor(x => x.Code).NotNull().NotEmpty();
        RuleFor(x => x.ExamId).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.ExamFileUpload).NotNull().NotEmpty().Must(BeAValidPdfFormat)
            .WithMessage("ExamEssayFile must be in a valid format (pdf).");
    }

    private bool BeAValidPdfFormat(IFormFile image)
    {
        var allowedExtensions = new[] { ".pdf" };
        var extension = Path.GetExtension(image.FileName).ToLower();
        return allowedExtensions.Contains(extension);
    }
}