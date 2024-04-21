using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comments.Commands.CreateComment
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, ErrorOr<SuccessResult>>
    {

        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IEmailService _emailService;

        public CreateCommentCommandHandler(IApplicationDbContext context,
            IMapper mapper,
            ICurrentUserProvider currentUserProvider,
            IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _currentUserProvider = currentUserProvider;
            _emailService = emailService;
        }

        public async Task<ErrorOr<SuccessResult>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            //Get current user
            var commentator = _currentUserProvider.GetCurrentUser();

            if (commentator == null) return Error.Unauthorized(description: "You are not authorized to this resource");

            //Finding contribution with the input Id and it must be belong to the current commentator faculty
            var contribution = await _context.Contributions
                .Include(c => c.CreatedBy).ThenInclude(u => u.Faculty)
                .FirstOrDefaultAsync(c => c.Id == request.ContributionId && c.CreatedBy.FacultyId == commentator.FacultyId, cancellationToken);

            if (contribution == null) return Error.NotFound(description: "Contribution not found");


            //Map and Add new comment into database 
            var commentEntity = _mapper.Map<Comment>(request);
            await _context.Comments.AddAsync(commentEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);


            return new SuccessResult(title: "Created a comment successfully");
        }
    }
}
