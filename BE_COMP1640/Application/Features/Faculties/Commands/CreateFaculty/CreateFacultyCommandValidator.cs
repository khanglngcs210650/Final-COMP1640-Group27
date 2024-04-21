using FluentValidation;

namespace Application.Features.Faculties.Commands.CreateFaculty;

public class CreateFacultyCommandValidator : AbstractValidator<CreateFacultyCommand>
{
    public CreateFacultyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}