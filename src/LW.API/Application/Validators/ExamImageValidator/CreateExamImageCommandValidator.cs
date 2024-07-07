﻿using FluentValidation;
using LW.Shared.DTOs;

namespace LW.API.Application.Validators.ExamImageValidator;

public class CreateExamImageCommandValidator :AbstractValidator<ExamImageCreateDto>
{
    public CreateExamImageCommandValidator()
    {
        RuleFor(x => x.ExamId).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.ImageUpload).NotNull().NotEmpty().Must(BeAValidImageFormat)
            .WithMessage("Each image must be in a valid format (.jpg, .jpeg, .png).");
    }
    private bool BeAValidImageFormat(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        return allowedExtensions.Contains(extension);
    }
}