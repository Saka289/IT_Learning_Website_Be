using FluentValidation;
using LW.Shared.DTOs.ExamAnswer;

namespace LW.API.Application.Validators.ExamAnswerValidator;

public class AnswerUpdateDtoValidator : AbstractValidator<AnswerUpdateDto>
{
    public AnswerUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.")
            .GreaterThan(0);
        RuleFor(x => x.NumberOfQuestion)
            .NotEmpty().WithMessage("NumberOfQuestion is required.")
            .GreaterThan(0).WithMessage("NumberOfQuestion must be greater than 0.");
        
        RuleFor(x => x.Answer)
            .NotEmpty().WithMessage("Answer is required.")
            .Must(BeAValidLetter).WithMessage("Answer must be a single letter from A to Z.");
    }
    private bool BeAValidLetter(char answer)
    {
        return char.IsLetter(answer);
    }
}