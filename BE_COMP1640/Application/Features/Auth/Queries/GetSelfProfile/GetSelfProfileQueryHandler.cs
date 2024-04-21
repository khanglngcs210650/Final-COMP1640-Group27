using Application.Common.Interfaces;
using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Queries.GetSelfProfile
{
    public class GetSelfProfileQueryHandler : IRequestHandler<GetSelfProfileQuery, ErrorOr<GetSelfProfileDto>>
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetSelfProfileQueryHandler(ICurrentUserProvider currentUserProvider,
            IApplicationDbContext context,
            IMapper mapper)
        {
            _currentUserProvider = currentUserProvider;
            _context = context;
            _mapper = mapper;
        }
        public async Task<ErrorOr<GetSelfProfileDto>> Handle(GetSelfProfileQuery request, CancellationToken cancellationToken)
        {
            var currentUser = _currentUserProvider.GetCurrentUser();
            if (currentUser == null) return Error.Unauthorized(description: "You must login first");
            var userInfo = await _context.Users
                .Include(u => u.Faculty)
                .Include(u => u.Contributions)
                .Include(u => u.Avatar)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == currentUser.Email, cancellationToken);
            if (userInfo == null) return Error.Unauthorized(description: "Invalid token information");
            return _mapper.Map<GetSelfProfileDto>(userInfo);
        }
    }
}
