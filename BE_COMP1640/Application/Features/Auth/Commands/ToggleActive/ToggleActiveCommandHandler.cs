using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands.ToggleActive
{
    public class ToggleActiveCommandHandler : IRequestHandler<ToggleActiveCommand, ErrorOr<SuccessResult>>
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ToggleActiveCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<ErrorOr<SuccessResult>> Handle(ToggleActiveCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null) return Error.NotFound(description: "User with given email not found");

            user.IsActive = !user.IsActive;

            await _userManager.UpdateAsync(user);

            string action = user.IsActive ? "activated" : "deactivated";

            return new SuccessResult(title: $"{action} user successfully!");

        }
    }
}
