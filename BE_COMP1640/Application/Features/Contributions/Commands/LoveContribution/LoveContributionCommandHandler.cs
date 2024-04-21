using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Contributions.Commands.LoveContribution
{
    public class LoveContributionCommandHandler : IRequestHandler<LoveContributionCommand, ErrorOr<SuccessResult>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserProvider _currentUserProvider;

        public LoveContributionCommandHandler(IApplicationDbContext context, ICurrentUserProvider currentUserProvider)
        {
            _context = context;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<ErrorOr<SuccessResult>> Handle(LoveContributionCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _currentUserProvider.GetCurrentUser();
            if (currentUser == null) return Error.Unauthorized(description: "You are not authorized to this resource");

            var contribution = await _context.Contributions
                .Include(c => c.CreatedBy)
                .Include(c => c.Ratings)
                .FirstOrDefaultAsync(c => c.Id == request.Id &&
                                          c.CreatedBy.FacultyId == currentUser.FacultyId &&
                                          (c.Status == ContributionStatus.Approved ||
                                           c.Status == ContributionStatus.Published),
                    cancellationToken);

            if (contribution == null) return Error.NotFound(description: "Contribution not found");

            // Check if the user has loved this contribution before
            var existingRating = contribution.Ratings.FirstOrDefault(r => r.CreatedById == currentUser.Id);

            // If the user has not loved this contribution yet, create a new rating and set loved to true
            if (existingRating == null)
            {
                contribution.Ratings.Add(new Rating
                {
                    Loved = true
                });
            }
            else
            {
                // If the user has loved this contribution before, toggle the love status
                existingRating.Loved = !existingRating.Loved;
            }

            // Save the changes to the database
            await _context.SaveChangesAsync(cancellationToken);


            return new SuccessResult(title: "Toggle contribution love successfully!");


        }
    }
}
