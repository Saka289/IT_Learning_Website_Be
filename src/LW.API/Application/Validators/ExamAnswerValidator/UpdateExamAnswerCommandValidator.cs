using FluentValidation;
using LW.Shared.DTOs.ExamAnswer;

namespace LW.API.Application.Validators.ExamAnswerValidator;

public class UpdateExamAnswerCommandValidator : AbstractValidator<ExamAnswerUpdateDto>
{
    public UpdateExamAnswerCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.NumberOfQuestion).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Answer).NotNull().NotEmpty().Must(BeAValidLetter)
            .WithMessage("Answer must be a single letter from A to Z.");
        RuleFor(x => x.ExamCodeId).NotNull().NotEmpty().GreaterThan(0);
    }
    private bool BeAValidLetter(char answer)
    {
        return char.IsLetter(answer);
    }
}