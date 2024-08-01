using FluentValidation;
using LW.Shared.DTOs.Tag;

namespace LW.API.Application.Validators.TagValidatior;

public class CreateTagCommandValidator : AbstractValidator<TagCreateDto>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.KeyWord).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotNull();
    }
}