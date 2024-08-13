using FluentValidation;
using LW.Shared.DTOs.Topic;

namespace LW.API.Application.Validators.TopicValidator;

public class CreateTopicCommandValidator:AbstractValidator<TopicCreateDto>
{
    public CreateTopicCommandValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty().Length(5,200);
        RuleFor(x => x.Description).NotNull().NotEmpty().Length(5, 200);
        RuleFor(x => x.Objectives).NotNull().NotEmpty().Length(5, 200);
        RuleFor(x => x.IsActive).NotEmpty();
        RuleFor(x => x.DocumentId).NotNull().NotEmpty().GreaterThan(0);
    }
}