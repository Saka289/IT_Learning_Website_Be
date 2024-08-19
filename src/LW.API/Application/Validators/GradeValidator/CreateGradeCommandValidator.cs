using System.Data;
using FluentValidation;
using LW.Shared.DTOs.Grade;

namespace LW.API.Application.Validators.GradeValidator;

public class CreateGradeCommandValidator : AbstractValidator<GradeCreateDto>
{
    public CreateGradeCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotEmpty();
    }
}