using FluentValidation;

namespace Application.Features.Periods.Commands.CreatePeriod;

public class CreatePeriodCommandHandlerValidator : AbstractValidator<CreatePeriodCommand>
{
    public CreatePeriodCommandHandlerValidator()
    {
        RuleFor(x => x.AcademicYear)
            .NotEmpty().WithMessage("Academic year must be provided.")
            .GreaterThanOrEqualTo(DateTime.Now.Year).WithMessage("Academic year must be the current year or later.");

        RuleFor(x => x.AcademicYear)
            .NotEmpty().WithMessage("Academic year must be provided.")
            .GreaterThanOrEqualTo(DateTime.Now.Year).WithMessage("Academic year must be the current year or later.");

        RuleFor(x => x.FirstSubmissionDeadline)
            .NotEmpty().WithMessage("First submission deadline must be provided.")
            .Must((command, firstDeadline) => IsWithinAcademicYear(command.AcademicYear, firstDeadline))
            .WithMessage("First submission deadline must be within the selected academic year.");

        RuleFor(x => x.SecondSubmissionDeadline)
            .NotEmpty().WithMessage("Second submission deadline must be provided.")
            .Must((command, secondDeadline) => IsWithinAcademicYear(command.AcademicYear, secondDeadline))
            .WithMessage("Second submission deadline must be within the selected academic year.")
            .GreaterThan(x => x.FirstSubmissionDeadline).WithMessage("Second submission deadline must be after the first submission deadline.");
    }

    private bool BeFutureDateTime(DateTime dateTime)
    {
        return dateTime > DateTime.Now;
    }

    private bool IsWithinAcademicYear(int academicYear, DateTime date)
    {
        return date.Year == academicYear;
    }
}