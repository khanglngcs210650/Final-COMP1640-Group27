using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Commands.ChangeFaculty
{
    public class ChangeFacultyCommandHandler : IRequestHandler<ChangeFacultyCommand, ErrorOr<SuccessResult>>
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChangeFacultyCommandHandler(UserManager<ApplicationUser> userManager,
            IApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<ErrorOr<SuccessResult>> Handle(ChangeFacultyCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null) return Error.NotFound(description: "User with the given email not found");

            var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Id == request.FacultyId, cancellationToken);

            if (faculty == null) return Error.NotFound(description: "Faculty with the given id not found");

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains("Admin")) return Error.Validation(description: "Cannot change faculty of Admin");


            if (userRoles.Contains("Manager")) return Error.Validation(description: "Cannot change faculty of Manager");

            user.FacultyId = request.FacultyId;

            await _userManager.UpdateAsync(user);

            return new SuccessResult(title: "Change faculty successfully!");
        }
    }
}
