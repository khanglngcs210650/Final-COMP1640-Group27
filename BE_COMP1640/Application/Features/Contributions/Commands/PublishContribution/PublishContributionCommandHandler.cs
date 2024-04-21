using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;
using ErrorOr;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Contributions.Commands.PublishContribution;

public class PublishContributionCommandHandler : IRequestHandler<PublishContributionCommand, ErrorOr<SuccessResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public PublishContributionCommandHandler(IApplicationDbContext context,
        IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }
    public async Task<ErrorOr<SuccessResult>> Handle(PublishContributionCommand request, CancellationToken cancellationToken)
    {
        var contribution = await _context.Contributions
            .Include(c => c.CreatedBy)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (contribution == null)
            return Error.NotFound(description: "Contribution not found");

        string action;
        if (request.Published && contribution.Status == ContributionStatus.Approved)
        {
            contribution.Status = ContributionStatus.Published;
            action = "Published";
        }
        else if (!request.Published && contribution.Status == ContributionStatus.Published)
        {
            contribution.Status = ContributionStatus.Approved;
            action = "Unpublished";
        }
        else
        {
            return Error.Validation(description: "Invalid operation");
        }

        await _context.SaveChangesAsync(cancellationToken);

        var contributor = contribution.CreatedBy;

        string emailSubject = $"Your contribution has been {action} by the Coordinator";
        string emailBody = $"Hi <strong>{contributor.FirstName} {contributor.LastName}</strong>," +
                           $"Your contribution with the title: <strong>{contribution.Title}</strong> has been <strong>{action}</strong> by your faculty <strong>Coordinator</strong>";

        BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(contributor.Email, emailSubject, emailBody));

        string statusMessage = $"Contribution {action} successfully!";
        return new SuccessResult(title: statusMessage);
    }
}