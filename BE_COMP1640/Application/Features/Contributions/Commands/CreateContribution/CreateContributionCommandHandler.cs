using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using ErrorOr;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Contributions.Commands.CreateContribution;

public class CreateContributionCommandHandler : IRequestHandler<CreateContributionCommand, ErrorOr<SuccessResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly IEmailService _emailService;
    private readonly ICurrentUserProvider _currentUserProvider;


    public CreateContributionCommandHandler(IApplicationDbContext context,
        IMapper mapper,
        IFileService fileService,
        IEmailService emailService,
        ICurrentUserProvider currentUserProvider)
    {
        _context = context;
        _mapper = mapper;
        _fileService = fileService;
        _emailService = emailService;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<ErrorOr<SuccessResult>> Handle(CreateContributionCommand request, CancellationToken cancellationToken)
    {
        var period = await _context.Periods.FirstOrDefaultAsync(p => p.Id == request.PeriodId, cancellationToken);

        if (period == null) return Error.NotFound(description: "Period with the given Id not found");

        if (DateTime.UtcNow > period.FirstSubmissionDeadline)
            return Error.Validation(description: "First submission deadline of this period has passed. You cannot submit a mre contribution");

        //Mapping and make status as Submitted
        var contributionEntity = _mapper.Map<Contribution>(request);
        contributionEntity.Status = ContributionStatus.Processing;

        //Save Image file and create Media entity
        var imageEntity = await _fileService.SaveFileAsync(request.ImageFile, "Images");
        //Save Document file and create Media entity
        var documentEntity = await _fileService.SaveFileAsync(request.DocumentFile, "Documents");

        //Add Media entities
        await _context.Media.AddRangeAsync([imageEntity, documentEntity], cancellationToken);

        //Connect FK
        contributionEntity.DocumentId = documentEntity.Id;
        contributionEntity.ImageId = imageEntity.Id;


        await _context.Contributions.AddAsync(contributionEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var currentUser = _currentUserProvider.GetCurrentUser();

        //Sending email to all the coordinator of the faculty that the contributor belongs to
        var coordinatorsToSendEmail = await _context.Users
            .Include(u => u.Roles)
            .Where(u => u.FacultyId == currentUser.FacultyId && u.Roles.Any(r => r.NormalizedName == "COORDINATOR"))
            .ToListAsync(cancellationToken);

        foreach (var coordinator in coordinatorsToSendEmail)
        {
            BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(coordinator.Email,
                "New contribution SUBMITTED",
                $"Hi <strong>{coordinator.FirstName} {coordinator.LastName}</strong>" +
                $"<p>A new contribution belong to your faculty has been <strong>SUBMITTED</strong> by <strong>{currentUser.Email}</strong> with the title: <strong>{contributionEntity.Title}</strong>. Please review and handle the contribution</p>"));
        }

        return new SuccessResult(title: "Submitted contribution successfully!");
    }
}