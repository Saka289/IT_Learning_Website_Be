using FluentValidation;
using LW.Shared.DTOs.Problem;

namespace LW.API.Application.Validators.ProblemValidator;

public class UpdateProblemCommandValidator : AbstractValidator<ProblemUpdateDto>
{
    public UpdateProblemCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.Difficulty).NotNull().NotEmpty().IsInEnum();
        RuleFor(x => x.IsActive).NotNull().NotEmpty();
    }
}