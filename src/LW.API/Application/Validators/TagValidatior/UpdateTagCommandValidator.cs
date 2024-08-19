using FluentValidation;
using LW.Shared.DTOs.Tag;

namespace LW.API.Application.Validators.TagValidatior;

public class UpdateTagCommandValidator:AbstractValidator<TagUpdateDto>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Title).NotNull().NotEmpty().Length(1, 20).WithMessage("Title must be between 1 and 20 characters."); 
        RuleFor(x => x.IsActive).NotNull();
    }
}