using Application.Common.Interfaces;
using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comments.Queries.ListComment
{
    public class ListCommentQueryHandler : IRequestHandler<ListCommentQuery, ErrorOr<IQueryable<ListCommentDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserProvider _currentUserProvider;

        public ListCommentQueryHandler(IApplicationDbContext context,
            IMapper mapper,
            ICurrentUserProvider currentUserProvider)
        {
            _context = context;
            _mapper = mapper;
            _currentUserProvider = currentUserProvider;
        }


        public Task<ErrorOr<IQueryable<ListCommentDto>>> Handle(ListCommentQuery request, CancellationToken cancellationToken)
        {
            var commentEntities = _context.Comments
                .Include(f => f.CreatedBy).ThenInclude(u => u.Faculty)
                .Include(f => f.Contribution)
                .AsNoTracking();

            var result = _mapper.ProjectTo<ListCommentDto>(commentEntities);

            return Task.FromResult(result.ToErrorOr());
        }
    }
}
