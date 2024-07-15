using FluentValidation;
using LW.Shared.DTOs.Tag;

namespace LW.API.Application.Validators.TagValidatior;

public class UpdateTagCommandValidator:AbstractValidator<TagUpdateDto>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.KeyWord).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotNull();
    }
}