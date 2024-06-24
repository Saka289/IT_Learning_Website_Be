using FluentValidation;
using LW.Shared.DTOs.Level;

namespace LW.API.Application.Validators.LevelValidator;

public class CreateLevelCommandValidator: AbstractValidator<LevelDtoForCreate>
{
    public CreateLevelCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotEmpty();
    }
}