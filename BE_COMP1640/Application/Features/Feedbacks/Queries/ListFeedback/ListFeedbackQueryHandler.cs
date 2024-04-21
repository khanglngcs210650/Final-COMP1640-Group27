using Application.Common.Interfaces;
using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Feedbacks.Queries.ListFeedback;

public class ListFeedbackQueryHandler : IRequestHandler<ListFeedbackQuery, ErrorOr<IQueryable<ListFeedbackDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserProvider _currentUserProvider;

    public ListFeedbackQueryHandler(IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserProvider currentUserProvider)
    {
        _context = context;
        _mapper = mapper;
        _currentUserProvider = currentUserProvider;
    }

    public Task<ErrorOr<IQueryable<ListFeedbackDto>>> Handle(ListFeedbackQuery request, CancellationToken cancellationToken)
    {
        var feedbackEntities = _context.Feedbacks
            .Include(f => f.CreatedBy).ThenInclude(u => u.Faculty)
            .Include(f => f.Contribution)
            .AsNoTracking();


        var user = _currentUserProvider.GetCurrentUser();

        //For Guest
        if (user == null) return Task.FromResult<ErrorOr<IQueryable<ListFeedbackDto>>>(Error.Forbidden(description: "You are not authorized to this resource"));
        //For authenticated user
        var roles = user.Roles;

        if (roles.Contains("Contributor"))
        {
            feedbackEntities = feedbackEntities.Where(c => c.Contribution.CreatedById == user.Id);
        }
        else if (roles.Contains("Coordinator"))
        {
            feedbackEntities = feedbackEntities.Where(c => c.Contribution.CreatedBy.FacultyId == user.FacultyId);
        }


        var result = _mapper.ProjectTo<ListFeedbackDto>(feedbackEntities);

        return Task.FromResult(result.ToErrorOr());
    }
}