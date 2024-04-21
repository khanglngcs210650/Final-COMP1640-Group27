using FluentValidation;

namespace Application.Features.Faculties.Commands.EditFaculty
{
    public class UpdateFacultyCommandValidator : AbstractValidator<UpdateFacultyCommand>
    {
        public UpdateFacultyCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        }
    }
}
