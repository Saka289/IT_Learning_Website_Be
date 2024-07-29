using FluentValidation;
using LW.Shared.DTOs.PostComment;

namespace LW.API.Application.Validators.PostCommentValidator;

public class UpdatePostCommentCommandValidator: AbstractValidator<PostCommentUpdateDto>
{
    public UpdatePostCommentCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Content).NotNull().NotEmpty();
        RuleFor(x => x.UserId).NotNull().NotEmpty();
        RuleFor(x => x.PostId).NotNull().NotEmpty().GreaterThan(0);
    }
}