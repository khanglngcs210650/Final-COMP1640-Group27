using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using ErrorOr;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Feedbacks.Commands.CreateFeedback;

public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, ErrorOr<SuccessResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IEmailService _emailService;

    public CreateFeedbackCommandHandler(IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserProvider currentUserProvider,
        IEmailService emailService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserProvider = currentUserProvider;
        _emailService = emailService;
    }

    public async Task<ErrorOr<SuccessResult>> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
    {
        //Get current coordinator
        var coordinator = _currentUserProvider.GetCurrentUser();

        if (coordinator == null) return Error.Unauthorized(description: "You are not authorized to this resource");

        //Finding contribution with the input Id and it must be belong to the current coordinator faculty
        var contribution = await _context.Contributions
            .Include(c => c.CreatedBy).ThenInclude(u => u.Faculty)
            .FirstOrDefaultAsync(c => c.Id == request.ContributionId && c.CreatedBy.FacultyId == coordinator.FacultyId, cancellationToken);

        if (contribution == null) return Error.NotFound(description: "Contribution not found");

        //Extract the contributor of the contribution
        var contributor = contribution.CreatedBy;

        //Map and Add new feedback into database 
        var feedbackEntity = _mapper.Map<Feedback>(request);
        await _context.Feedbacks.AddAsync(feedbackEntity, cancellationToken);


        if (contribution.Status == ContributionStatus.Processing)
        {
            contribution.Status = ContributionStatus.Processed;
        }

        await _context.SaveChangesAsync(cancellationToken);

        BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(
            contributor.Email,
           "New Feedback on your Contribution",
           $"Hi {contributor.FirstName} {contributor.LastName}," +
                  $"<br/><br/>Your faculty coordinator with the email: <strong>{coordinator.Email}</strong> has provided new feedback on your contribution with the title <strong>{contribution.Title}</strong>." +
                  $"<br/><br/>Please read the feedback carefully and improve your contribution to match the requirements!"
        ));


        return new SuccessResult(title: "Created a feedback successfully");

    }
}