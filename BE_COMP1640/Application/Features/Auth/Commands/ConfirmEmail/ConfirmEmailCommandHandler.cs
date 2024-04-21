using Domain.Entities;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, ErrorOr<Success>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ErrorOr<Success>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Error.NotFound(description: "Email not found");

        if (user.EmailConfirmed) return Error.Conflict(description: "This email is confirmed");

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        if (result.Succeeded == false) return Error.Unauthorized(description: "Token is invalid");


        return Result.Success;
    }
}