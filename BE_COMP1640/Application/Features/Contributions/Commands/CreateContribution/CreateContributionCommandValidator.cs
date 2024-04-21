using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Contributions.Commands.CreateContribution;

public class CreateContributionCommandValidator : AbstractValidator<CreateContributionCommand>
{
    public CreateContributionCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.ImageFile)
            .NotEmpty().WithMessage("Image file is required.")
            .Must(BeAValidImageFile).WithMessage("Unsupported image file extension. Support extensions: .jpg, .jpeg, .png, .webp")
            .Must(BeUnder20Mb).WithMessage("Image file size should be less than 10MB.");

        RuleFor(x => x.DocumentFile)
            .NotEmpty().WithMessage("Document file is required.")
            .Must(BeAValidDocumentFile).WithMessage("Unsupported document file extension. Support extensions: .doc, .docx, .pdf")
            .Must(BeUnder20Mb).WithMessage("Document file size should be less than 10MB.");

        RuleFor(x => x.PeriodId)
            .NotEmpty().WithMessage("PeriodId is required");

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