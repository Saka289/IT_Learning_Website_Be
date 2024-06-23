using FluentValidation;
using LW.Shared.DTOs.Document;

namespace LW.API.Application.Validators.DocumentValidator;

public class CreateDocumentCommandValidator : AbstractValidator<DocumentCreateDto>
{
    public CreateDocumentCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotEmpty();
        RuleFor(x => x.GradeId).NotNull().NotEmpty().GreaterThan(0);
    }
}