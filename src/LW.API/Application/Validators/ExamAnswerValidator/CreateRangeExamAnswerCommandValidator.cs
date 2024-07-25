using FluentValidation;
using LW.Shared.DTOs.ExamAnswer;

namespace LW.API.Application.Validators.ExamAnswerValidator;

public class CreateRangeExamAnswerCommandValidator : AbstractValidator<ExamAnswerCreateRangeDto>
{
    public CreateRangeExamAnswerCommandValidator()
    {
        RuleFor(x => x.ExamCodeId)
            .NotEmpty().WithMessage("ExamCodeId is required.")
            .NotNull().WithMessage("ExamCodeId cannot be null.")
            .GreaterThan(0).WithMessage("ExamCodeId must be greater than 0.");
        
        RuleForEach(x => x.AnswerDtos).SetValidator(new AnswerDtoValidator());
    }
}