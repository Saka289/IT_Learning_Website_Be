using FluentValidation;
using LW.Shared.DTOs.Document;
using LW.Shared.Enums;

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
        RuleFor(x => x.PublicationYear)
            .NotNull()
            .NotEmpty()
            .InclusiveBetween(1900, DateTime.Now.Year)
            .WithMessage($"Publication year must be between 1900 and {DateTime.Now.Year}.");
        RuleFor(x => x.BookCollection).NotNull().NotEmpty().IsInEnum();
        RuleFor(x => x.TypeOfBook).NotNull().NotEmpty().IsInEnum();
        RuleFor(x => x.Edition).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Image).NotNull().NotEmpty();
    }
}