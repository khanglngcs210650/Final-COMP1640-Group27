using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Auth.Commands.UpdateProfile;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters");

        RuleFor(x => x.LastName)
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");

        RuleFor(x => x.AvatarFile)
            .Must(BeAValidImageFile).WithMessage("Unsupported image file extension. Supported extensions: .jpg, .jpeg, .png, .webp")
            .Must(BeUnder20Mb).WithMessage("Image file size should be less than 10MB.")
            .When(x => x.AvatarFile != null);
    }

    private bool BeAValidImageFile(IFormFile? file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        return file != null && allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower());
    }


    private bool BeUnder20Mb(IFormFile? file)
    {
        return file != null && file.Length <= 20971520;
    }
}