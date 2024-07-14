using FluentValidation;
using LW.Shared.DTOs.ExamAnswer;

namespace LW.API.Application.Validators.ExamAnswerValidator;

public class CreateRangeExamAnswerCommandValidator : AbstractValidator<ExamAnswerCreateRangeDto>
{
    public CreateRangeExamAnswerCommandValidator()
    {
        RuleFor(x => x.ExamId)
            .NotEmpty().WithMessage("ExamId is required.")
            .NotNull().WithMessage("ExamId cannot be null.")
            .GreaterThan(0).WithMessage("ExamId must be greater than 0.");
        
        RuleForEach(x => x.AnswerDtos).SetValidator(new AnswerDtoValidator());
    }
}