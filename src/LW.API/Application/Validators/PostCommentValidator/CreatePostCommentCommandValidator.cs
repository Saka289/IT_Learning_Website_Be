using FluentValidation;
using LW.Shared.DTOs.PostComment;

namespace LW.API.Application.Validators.PostCommentValidator;

public class CreatePostCommentCommandValidator : AbstractValidator<PostCommentCreateDto>
{
    public CreatePostCommentCommandValidator()
    {
        RuleFor(x => x.Content).NotNull().NotEmpty();
        RuleFor(x => x.UserId).NotNull().NotEmpty();
        RuleFor(x => x.PostId).NotNull().NotEmpty().GreaterThan(0);
    }
}