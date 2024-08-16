using FluentValidation;
using LW.Shared.DTOs.Topic;

namespace LW.API.Application.Validators.TopicValidator;

public class UpdateTopicCommandValidator:AbstractValidator<TopicUpdateDto>
{
    public UpdateTopicCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Title).NotNull().NotEmpty().Length(5, 200);
        RuleFor(x => x.Description).NotNull().NotEmpty().Length(5, 200);
        RuleFor(x => x.Objectives).NotNull().NotEmpty().Length(5, 200);
        RuleFor(x => x.IsActive).NotNull();
        RuleFor(x => x.DocumentId).NotNull().NotEmpty().GreaterThan(0);
    }
}