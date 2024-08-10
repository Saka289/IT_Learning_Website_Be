using FluentValidation;
using LW.Shared.DTOs.CommentDocument;
using LW.Shared.DTOs.Exam;

namespace LW.API.Application.Validators.ExamValidator;

public class UpdateExamCommandValidator:AbstractValidator<ExamUpdateDto>
{
    public UpdateExamCommandValidator()
    {
        RuleFor(x => x.CompetitionId).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Title).NotNull().NotEmpty().Length(5, 100)
            .WithMessage("Title must be between 5 and 100 characters.");
        RuleFor(x => x.Type).NotNull().NotEmpty();
        RuleFor(x => x.Province).NotNull().NotEmpty();
        RuleFor(x => x.ExamEssayFileUpload).Must(BeAValidPdfFormat)
            .WithMessage("ExamEssayFileUpload must be in a valid format (pdf).").When(x=>x.ExamEssayFileUpload != null);
        RuleFor(x => x.ExamSolutionFileUpload).Must(BeAValidPdfFormat)
            .WithMessage("ExamSolutionFileUpload must be in a valid format (pdf).").When(x=>x.ExamSolutionFileUpload != null);
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.Year).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.NumberQuestion).NotNull().GreaterThan(-1);
        RuleFor(x => x.IsActive);
        RuleFor(x => x.GradeId).GreaterThan(0);
    }
    private bool BeAValidPdfFormat(IFormFile file)
    {
        var allowedExtensions = new[] { ".pdf" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        return allowedExtensions.Contains(extension);
    }
}