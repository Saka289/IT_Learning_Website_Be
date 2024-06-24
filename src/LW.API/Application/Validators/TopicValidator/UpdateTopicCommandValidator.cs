using FluentValidation;
using LW.Shared.DTOs.Topic;

namespace LW.API.Application.Validators.TopicValidator;

public class UpdateTopicCommandValidator:AbstractValidator<TopicUpdateDto>
{
    public UpdateTopicCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Title).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
        RuleFor(x => x.Objectives).NotNull().NotEmpty();
        RuleFor(x => x.IsActive).NotEmpty();
        RuleFor(x => x.DocumentId).NotNull().NotEmpty().GreaterThan(0);
    }
}