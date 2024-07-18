using FluentValidation;
using LW.Data.Entities;
using LW.Shared.DTOs.ExamAnswer;

namespace LW.API.Application.Validators.ExamAnswerValidator;

public class CreateExamAnswerCommandValidator: AbstractValidator<ExamAnswerCreateDto>
{
    public CreateExamAnswerCommandValidator()
    {
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