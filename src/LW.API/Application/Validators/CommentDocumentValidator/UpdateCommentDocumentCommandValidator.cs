﻿using FluentValidation;
using LW.Shared.DTOs.CommentDocumentDto;

namespace LW.API.Application.Validators.CommentDocumentValidator;

public class UpdateCommentDocumentCommandValidator : AbstractValidator<CommentDocumentUpdateDto>
{
    public UpdateCommentDocumentCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
        RuleFor(x => x.Note).NotNull().NotEmpty();
        RuleFor(x => x.Rating).NotNull().NotEmpty().GreaterThan(0).LessThan(6);
        RuleFor(x => x.DocumentId).NotNull().NotEmpty();
        RuleFor(x => x.UserId).NotNull().NotEmpty();
    }
}