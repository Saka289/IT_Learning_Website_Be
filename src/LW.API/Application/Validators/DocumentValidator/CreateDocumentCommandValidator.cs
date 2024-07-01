using FluentValidation;
using LW.Shared.DTOs.Document;

namespace LW.API.Application.Validators.DocumentValidator;

public class CreateDocumentCommandValidator : AbstractValidator<DocumentCreateDto>
{
    public CreateDocumentCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotNull();
        RuleFor(x => x.GradeId).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Author).NotNull().NotEmpty();
        RuleFor(x => x.PublicationYear).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.BookCollection).NotNull().NotEmpty();
        RuleFor(x => x.TypeOfBook).NotNull().NotEmpty();
        RuleFor(x => x.Edition).NotNull().NotEmpty().GreaterThan(0);

    }
}