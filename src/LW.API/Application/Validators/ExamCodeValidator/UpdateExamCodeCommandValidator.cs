using FluentValidation;
using LW.Shared.DTOs.ExamCode;

namespace LW.API.Application.Validators.ExamCodeValidator;

public class UpdateExamCodeCommandValidator: AbstractValidator<ExamCodeUpdateDto>
{
    public UpdateExamCodeCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Code).NotNull().NotEmpty();
        RuleFor(x => x.ExamId).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.ExamFileUpload).Must(BeAValidPdfFormat)
            .WithMessage("ExamEssayFileUpload must be in a valid format (pdf).").When(x=>x.ExamFileUpload != null);
    }
    private bool BeAValidPdfFormat(IFormFile image)
    {
        var allowedExtensions = new[] { ".pdf" };
        var extension = Path.GetExtension(image.FileName).ToLower();
        return allowedExtensions.Contains(extension);
    }
}