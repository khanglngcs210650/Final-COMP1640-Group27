using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Periods.Commands.UpdatePeriod;

public class UpdatePeriodCommandHandlerValidator : AbstractValidator<UpdatePeriodCommand>
{

    private readonly IApplicationDbContext _context;

    public UpdatePeriodCommandHandlerValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Id)
            .MustAsync(IdExists).WithMessage("Period not found").WithErrorCode("NotFound")
            .DependentRules(() =>
            {
                RuleFor(x => x.FirstSubmissionDeadline)
                    .NotEmpty().WithMessage("First submission deadline must be provided.")
                    .MustAsync(async (command, firstDeadline, cancellationToken) =>
                    {
                        var academicYear = await GetAcademicYear(command.Id, cancellationToken);
                        return IsWithinAcademicYear(academicYear, firstDeadline);
                    })
                    .WithMessage("First submission deadline must be within the selected academic year.");

                RuleFor(x => x.SecondSubmissionDeadline)
                    .NotEmpty().WithMessage("Second submission deadline must be provided.")
                    .MustAsync(async (command, secondDeadline, cancellationToken) =>
                    {
                        var academicYear = await GetAcademicYear(command.Id, cancellationToken);
                        return IsWithinAcademicYear(academicYear, secondDeadline);
                    })
                    .WithMessage("Second submission deadline must be within the selected academic year.")
                    .GreaterThan(x => x.FirstSubmissionDeadline).WithMessage("Second submission deadline must be after the first submission deadline.");
            });
    }

    private Task<bool> IdExists(Guid id, CancellationToken cancellationToken)
    {
        return _context.Periods.AnyAsync(p => p.Id == id, cancellationToken);
    }

    private async Task<int> GetAcademicYear(Guid id, CancellationToken cancellationToken)
    {
        var period = await _context.Periods.FindAsync(id);
        if (period != null)
        {
            return period.AcademicYear;
        }
        return 0;
    }

    private bool IsWithinAcademicYear(int academicYear, DateTime date)
    {
        return date.Year == academicYear;
    }


}