using FluentValidation;
using LW.Shared.DTOs.Post;

namespace LW.API.Application.Validators.PostValidator;

public class UpdatePostCommandValidator: AbstractValidator<PostUpdateDto>
{
    public UpdatePostCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.UserId).NotNull().NotEmpty();
        RuleFor(x => x.Content).NotNull().NotEmpty().Length(10, 500)
            .WithMessage("Content must be between 10 and 500 characters.");
        RuleFor(x => x.GradeId).NotNull().NotEmpty().GreaterThan(0);
    }
}