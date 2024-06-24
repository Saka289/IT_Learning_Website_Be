using FluentValidation;
using LW.Shared.DTOs.Level;

namespace LW.API.Application.Validators.LevelValidator;

public class UpdateLevelCommandValidator:AbstractValidator<LevelDtoForUpdate>
{
    public UpdateLevelCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotEmpty();
    }
}