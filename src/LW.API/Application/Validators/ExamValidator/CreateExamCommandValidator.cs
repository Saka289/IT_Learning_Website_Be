using FluentValidation;
using LW.Shared.DTOs.CommentDocument;
using LW.Shared.DTOs.Exam;

namespace LW.API.Application.Validators.ExamValidator;

public class CreateExamCommandValidator:AbstractValidator<ExamCreateDto>
{
    public CreateExamCommandValidator()
    {
        RuleFor(x => x.CompetitionId)
            .NotNull()
            .NotEmpty()
            .GreaterThan(0);
        RuleFor(x => x.Type).NotNull().NotEmpty().IsInEnum();
        RuleFor(x => x.Title).NotNull().NotEmpty().Length(5, 100)
                                                          .WithMessage("Title must be between 5 and 100 characters.");;
        RuleFor(x => x.Province).NotNull().NotEmpty();
        RuleFor(x => x.ExamEssayFileUpload).Must(BeAValidPdfFormat).When(x=>x.ExamEssayFileUpload != null)
            .WithMessage("ExamEssayFile must be in a valid format (pdf).");
        RuleFor(x => x.ExamSolutionFileUpload).Must(BeAValidPdfFormat).When(x=>x.ExamSolutionFileUpload != null)
            .WithMessage("ExamSolutionFileUpload must be in a valid format (pdf).");
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.Year).NotNull().NotEmpty().InclusiveBetween(0, DateTime.Now.Year + 1)
            .WithMessage($"Year must be between 0 and {DateTime.Now.Year + 1}.");;
        RuleFor(x => x.NumberQuestion).NotNull().GreaterThan(-1);
        RuleFor(x => x.IsActive).NotNull();
        RuleFor(x => x.tagValues).NotNull().NotEmpty();
        
    }
    private bool BeAValidPdfFormat(IFormFile image)
    {
        var allowedExtensions = new[] { ".pdf" };
        var extension = Path.GetExtension(image.FileName).ToLower();
        return allowedExtensions.Contains(extension);
    }
    // private bool BeAValidImageFormat(IFormFile file)
    // {
    //     var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
    //     var extension = Path.GetExtension(file.FileName).ToLower();
    //     return allowedExtensions.Contains(extension);
    // }
}