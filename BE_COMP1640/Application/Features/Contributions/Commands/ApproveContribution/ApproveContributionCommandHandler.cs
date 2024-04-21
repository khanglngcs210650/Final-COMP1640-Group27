using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;
using ErrorOr;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Contributions.Commands.ApproveContribution;

public class ApproveContributionCommandHandler : IRequestHandler<ApproveContributionCommand, ErrorOr<SuccessResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ICurrentUserProvider _currentUserProvider;

    public ApproveContributionCommandHandler(IApplicationDbContext context,
        IEmailService emailService,
        ICurrentUserProvider currentUserProvider)
    {
        _context = context;
        _emailService = emailService;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<ErrorOr<SuccessResult>> Handle(ApproveContributionCommand request, CancellationToken cancellationToken)
    {
        var coordinator = _currentUserProvider.GetCurrentUser();
        if (coordinator == null) return Error.Unauthorized(description: "You are not authorize to this");

        var contribution = await _context.Contributions
            .Include(c => c.CreatedBy)
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CreatedBy.FacultyId == coordinator.FacultyId, cancellationToken);

        if (contribution == null) return Error.NotFound(description: "Contribution not found");

        var status = request.Approved ? ContributionStatus.Approved : ContributionStatus.Rejected;
        contribution.Status = status;

        await _context.SaveChangesAsync(cancellationToken);
        var contributor = contribution.CreatedBy;


        //Sending email notification to the Contributor
        string statusColor = request.Approved ? "green" : "red";
        string statusText = $"<span style=\"color: {statusColor};\"><strong>{status}</strong></span>";

        BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(contribution.CreatedBy.Email,
            $"Your contribution has been {status}",
            $"Hi <strong>{contributor.FirstName} {contributor.LastName}</strong>" +
            $"<p>Your contribution with the title: <strong>{contribution.Title}</strong> has been {statusText} by the your faculty Coordinator with the email: <strong>{coordinator.Email}</strong>"));

        //Return the result to the client
        return new SuccessResult(
            title: $"Contribution has been {(request.Approved ? "Approved" : "Rejected")} successfully!");

    }
}