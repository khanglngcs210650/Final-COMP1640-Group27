using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands.AssignRole;

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, ErrorOr<SuccessResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public AssignRoleCommandHandler(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }


    public async Task<ErrorOr<SuccessResult>> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null) return Error.NotFound(description: "User with the given email not found");

        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());

        if (role == null) return Error.NotFound(description: "Role with the given id not found");

        var userRoles = await _userManager.GetRolesAsync(user);

        if (userRoles.Contains("Admin")) return Error.Validation(description: "Admin role cannot be modified or reassigned");

        var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);

        if (!removeResult.Succeeded) return Error.Unexpected(description: "Failed to remove existing roles");

        var result = await _userManager.AddToRoleAsync(user, role.Name);

        if (role.Name == "Admin" || role.Name == "Manager")
        {
            user.FacultyId = null;
        }

        await _userManager.UpdateAsync(user);

        if (!result.Succeeded) return Error.Unexpected(description: "Failed to assign role, please try again");

        return new SuccessResult(title: "Assigned role to user successfully!");

    }
}