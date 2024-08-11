﻿using FluentValidation;
using LW.Shared.DTOs.Quiz;

namespace LW.API.Application.Validators.QuizValidator;

public class CreateQuizCommandValidator : AbstractValidator<QuizCreateDto>
{
    public CreateQuizCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty().Length(5,250);
        RuleFor(x => x.Description).NotNull().NotEmpty().Length(5, 250);
        RuleFor(x => x.Score).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Type).NotNull().NotEmpty().IsInEnum();
        RuleFor(x => x.IsActive).NotNull().NotEmpty();
    }
}