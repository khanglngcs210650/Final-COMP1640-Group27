using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Contributions.Commands.UpdateContribution;

public class UpdateContributionCommandValidator : AbstractValidator<UpdateContributionCommand>
{


    public UpdateContributionCommandValidator()
    {

        RuleFor(x => x.Title)
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");


        RuleFor(x => x.ImageFile)
            .Must(BeAValidImageFile).WithMessage("Unsupported image file extension. Supported extensions: .jpg, .jpeg, .png, .webp")
            .Must(BeUnder20Mb).WithMessage("Image file size should be less than 10MB.")
            .When(x => x.ImageFile != null);


        RuleFor(x => x.DocumentFile)
            .Must(BeAValidDocumentFile).WithMessage("Unsupported document file extension. Supported extensions: .doc, .docx, .pdf")
            .Must(BeUnder20Mb).WithMessage("Document file size should be less than 10MB.")
            .When(x => x.DocumentFile != null);
    }




    private bool BeAValidImageFile(IFormFile? file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        return file != null && allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower());
    }

    private bool BeAValidDocumentFile(IFormFile? file)
    {
        var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
        return file != null && allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower());
    }

    private bool BeUnder20Mb(IFormFile? file)
    {
        return file != null && file.Length <= 20971520;
    }


}