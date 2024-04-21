using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ErrorOr<SuccessResult>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }
    public async Task<ErrorOr<SuccessResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null) return Error.Unauthorized(description: "Email or password wrong");

        var checkPasswordResult = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!checkPasswordResult) return Error.Unauthorized(description: "Email or password wrong");

        if (user.EmailConfirmed == false) return Error.Unauthorized(description: "Please verify your email first!");

        if (user.IsActive == false) return Error.Unauthorized(description: "You are banned from the system due to the violation of Term of services");

        if (user.IsInitialPasswordChanged == false)
        {
            var changeInitialPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            return new SuccessResult(title: "You must change your initial password first", data: new { ChangeInitialPasswordToken = changeInitialPasswordToken });
        }

        var roles = await _userManager.GetRolesAsync(user);


        var token = _jwtTokenGenerator.GenerateToken(id: user.Id,
            facultyId: user.FacultyId,
            email: user.Email,
            firstName: user.FirstName,
            lastName: user.LastName,
            roles: roles.ToList());

        return new SuccessResult(title: "Login successfully!", data: token);
    }
}