using FluentValidation;
using LW.Shared.DTOs.CommentDocument;

namespace LW.API.Application.Validators.CommentDocumentValidator;

public class CreateCommentDocumentCommandValidator : AbstractValidator<CommentDocumentCreateDto>
{
    public CreateCommentDocumentCommandValidator()
    {
        RuleFor(x => x.Note).NotNull().NotEmpty();
        RuleFor(x => x.Rating).NotNull().NotEmpty().GreaterThan(0).LessThan(6);
        RuleFor(x => x.DocumentId).NotNull().NotEmpty();
        RuleFor(x => x.UserId).NotNull().NotEmpty();
    }
}