using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Enums;
using ErrorOr;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Contributions.Commands.UpdateContribution;

public class UpdateContributionCommandHandler : IRequestHandler<UpdateContributionCommand, ErrorOr<SuccessResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IEmailService _emailService;

    public UpdateContributionCommandHandler(IApplicationDbContext context,
        IMapper mapper,
        IFileService fileService,
        ICurrentUserProvider currentUserProvider,
        IEmailService emailService)
    {
        _context = context;
        _mapper = mapper;
        _fileService = fileService;
        _currentUserProvider = currentUserProvider;
        _emailService = emailService;
    }

    public async Task<ErrorOr<SuccessResult>> Handle(UpdateContributionCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserProvider.GetCurrentUser();

        if (currentUser == null) return Error.Unauthorized(description: "You are not authorized to this resource");

        var contributionEntity = await _context.Contributions
            .Include(c => c.Image)
            .Include(c => c.Document)
            .Include(c => c.Period)
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CreatedById == currentUser.Id, cancellationToken);

        if (contributionEntity == null) return Error.NotFound("Contribution not found");


        if (DateTime.UtcNow > contributionEntity.Period.SecondSubmissionDeadline)
            return Error.Validation(description: "Second submission deadline has passed. You cannot update this contribution.");

        _mapper.Map(request, contributionEntity);


        if (request.ImageFile != null) await _fileService.UpdateFileAsync(request.ImageFile, "Images", contributionEntity.Image);

        if (request.DocumentFile != null) await _fileService.UpdateFileAsync(request.DocumentFile, "Documents", contributionEntity.Document);

        if (contributionEntity.Status == ContributionStatus.Processed)
        {
            contributionEntity.Status = ContributionStatus.Processing;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var coordinatorsToSendEmail = await _context.Users
            .Include(u => u.Roles)
            .Where(u => u.FacultyId == currentUser.FacultyId && u.Roles.Any(r => r.NormalizedName == "COORDINATOR"))
            .ToListAsync(cancellationToken);

        foreach (var coordinator in coordinatorsToSendEmail)
        {
            BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(coordinator.Email,
                "Contribution has been UPDATED",
                $"Hi <strong>{coordinator.FirstName} {coordinator.LastName}</strong>" +
                $"<p>A contribution belong to your faculty has been <strong>UPDATED</strong> by <strong>{currentUser.Email}</strong> with the title: <strong>{contributionEntity.Title}</strong>. Please review and give new feedbacks</p>"));
        }


        return new SuccessResult(title: "Updated contribution successfully!");
    }
}