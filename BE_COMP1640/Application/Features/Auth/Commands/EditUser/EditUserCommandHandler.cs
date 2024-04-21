using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Commands.EditUser
{
    public class EditUserCommandHandler : IRequestHandler<EditUserCommand, ErrorOr<SuccessResult>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IEmailService _emailService;

        public EditUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IApplicationDbContext context,
            ICurrentUserProvider currentUserProvider,
            IEmailService emailService
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _currentUserProvider = currentUserProvider;
            _emailService = emailService;
        }
        public async Task<ErrorOr<SuccessResult>> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _currentUserProvider.GetCurrentUser();

            if (currentUser == null) return Error.Unauthorized(description: "You are not authorized to this resource");

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null) return Error.NotFound(description: "User with the given email not found");

            var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());

            if (role == null) return Error.NotFound(description: "Role with the given id not found");


            if ((role.Name == "Manager" || role.Name == "Admin") && request.FacultyId != null) return Error.Validation("You can't specify the faculty for Manager or Admin roles.");

            if ((role.Name == "Contributor" || role.Name == "Coordinator") && request.FacultyId == null) return Error.Validation("You must specify the faculty for Contributor or Coordinator roles.");

            if (request.FacultyId != null)
            {
                var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Id == request.FacultyId, cancellationToken);

                if (faculty == null) return Error.NotFound(description: "Faculty with the given FacultyId not found");

                user.FacultyId = request.FacultyId;

            }

            if (role.Name == "Admin" || role.Name == "Manager")
            {
                user.FacultyId = null;
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains("Admin")) return Error.Validation(description: "Admin role cannot be modified or reassigned");

            var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);

            if (!removeResult.Succeeded) return Error.Unexpected(description: "Failed to remove existing roles");

            var result = await _userManager.AddToRoleAsync(user, role.Name);

            if (!result.Succeeded) return Error.Unexpected(description: "Failed to assign role, please try again");

            await _userManager.UpdateAsync(user);



            return new SuccessResult(title: "Edit user successfully!");

        }
    }
}
