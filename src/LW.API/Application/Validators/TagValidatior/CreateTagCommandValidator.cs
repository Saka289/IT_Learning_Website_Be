using FluentValidation;
using LW.Shared.DTOs.Tag;

namespace LW.API.Application.Validators.TagValidatior;

public class CreateTagCommandValidator : AbstractValidator<TagCreateDto>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty().Length(1,20).WithMessage("Title must be between 1 and 20 characters.");
        RuleFor(x => x.IsActive).NotNull();
    }
}