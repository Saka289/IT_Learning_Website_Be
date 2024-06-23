using FluentValidation;
using LW.Shared.DTOs.Document;

namespace LW.API.Application.Validators.DocumentValidator;

public class UpdateDocumentCommandValidator : AbstractValidator<DocumentUpdateDto>
{
    public UpdateDocumentCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotEmpty();
        RuleFor(x => x.GradeId).NotNull().NotEmpty().GreaterThan(0);
    }
}