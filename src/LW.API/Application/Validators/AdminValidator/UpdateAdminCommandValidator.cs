using FluentValidation;
using LW.Shared.DTOs.Admin;

namespace LW.API.Application.Validators.AdminValidator;

public class UpdateAdminCommandValidator:AbstractValidator<UpdateAdminDto>
{
    public UpdateAdminCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().NotNull();
        RuleFor(x => x.FirstName).NotEmpty().NotNull();
        RuleFor(x => x.LastName).NotEmpty().NotNull();
        RuleFor(x => x.PhoneNumber).NotEmpty().NotNull();
        RuleFor(x => x.Dob).NotEmpty().NotNull();
        RuleFor(x => x.Image).NotEmpty().NotNull().Must(BeAValidImageFormat)
            .WithMessage("Image must be in a valid format (jpg, jpeg, png).");
    }
    private bool BeAValidImageFormat(IFormFile image)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(image.FileName).ToLower();
        return allowedExtensions.Contains(extension);
    }
}