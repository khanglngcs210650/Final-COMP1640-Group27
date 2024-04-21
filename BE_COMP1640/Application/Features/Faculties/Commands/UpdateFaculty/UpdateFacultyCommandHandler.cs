using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Faculties.Commands.EditFaculty;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Faculties.Commands.UpdateFaculty
{
    public class UpdateFacultyCommandHandler : IRequestHandler<UpdateFacultyCommand, ErrorOr<SuccessResult>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateFacultyCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<SuccessResult>> Handle(UpdateFacultyCommand request, CancellationToken cancellationToken)
        {
            var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

            if (faculty == null) return Error.NotFound(description: "Faculty not found");

            faculty.Name = request.Name;

            await _context.SaveChangesAsync(cancellationToken);

            return new SuccessResult(title: "Updated faculty successfully!");
        }
    }
}
