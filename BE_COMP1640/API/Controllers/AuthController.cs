using API.Sieve;
using Application.Features.Auth.Commands.AssignRole;
using Application.Features.Auth.Commands.ChangeFaculty;
using Application.Features.Auth.Commands.ChangeInitialPassword;
using Application.Features.Auth.Commands.ConfirmEmail;
using Application.Features.Auth.Commands.CreateAllAccount;
using Application.Features.Auth.Commands.CreateContributorAccount;
using Application.Features.Auth.Commands.CreateCoordinatorAccount;
using Application.Features.Auth.Commands.EditUser;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.ResetPassword;
using Application.Features.Auth.Commands.ToggleActive;
using Application.Features.Auth.Commands.UpdateProfile;
using Application.Features.Auth.Queries.GetResetPasswordOTP;
using Application.Features.Auth.Queries.GetSelfProfile;
using Application.Features.Auth.Queries.ListRole;
using Application.Features.Auth.Queries.ListUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using Sieve.Services;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ApiController
{
    private readonly ISender _sender;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthController(ISender sender, ISieveProcessor sieveProcessor,
        IHttpContextAccessor httpContextAccessor)
    {
        _sender = sender;
        _sieveProcessor = sieveProcessor;
        _httpContextAccessor = httpContextAccessor;
    }


    /// <summary>
    ///    Registering a new account
    /// </summary>
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request)
    {
        var authResult = await _sender.Send(request);

        return authResult.Match(
            value => base.StatusCode(201, value),
            Problem);
    }

    /// <summary>
    ///   Login to get JWT
    /// </summary>
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
        var authResult = await _sender.Send(request);

        return authResult.Match(
            value => base.Ok(value),
            Problem);
    }

    [HttpGet]
    [Route("SelfProfile")]
    [Authorize]
    public async Task<IActionResult> SelfProfile()
    {
        var result = await _sender.Send(new GetSelfProfileQuery());

        return result.Match(
            value => base.Ok(value),
            Problem);
    }


    /// <summary>
    ///   [MustAuthenticated]  Update oneself profile
    /// </summary>
    [HttpPut]
    [Route("UpdateProfile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileCommand command)
    {
        var result = await _sender.Send(command);

        return result.Match(
            value => base.Ok(value),
            Problem);
    }

    /// <summary>
    ///    (Ignore this) Just only for confirming email via sent email
    /// </summary>
    [HttpGet]
    [Route("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand command)
    {

        var result = await _sender.Send(command);

        return result.Match(
            value => base.Ok("Confirmed email successfully, now you can login"),
            Problem);
    }

    /// <summary>
    ///    Send Reset password OTP via Email
    /// </summary>
    [HttpPost]
    [Route("SendResetPasswordOTP")]
    public async Task<IActionResult> SendResetPasswordOTP([FromBody] GetResetPasswordOTPQuery request)
    {

        var result = await _sender.Send(request);

        return result.Match(
            value => base.Ok(value),
            Problem);
    }

    /// <summary>
    ///    Using sent OTP with new password to set a new password
    /// </summary>
    [HttpPost]
    [Route("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand request)
    {

        var result = await _sender.Send(request);

        return result.Match(
            value => base.Ok(value),
            Problem);
    }



    /// <summary>
    ///    List all users in the system
    /// </summary>
    [HttpGet]
    [Route("Users")]
    public async Task<IActionResult> ListUser([FromQuery] SieveModel sieveModel)
    {

        var result = await _sender.Send(new ListUserQuery());


        if (result.IsError)
        {
            return Problem(result.Errors);
        }


        return base.Ok(await result.Value.ToPaginatedListAsync(_sieveProcessor, sieveModel, _httpContextAccessor));

    }

    /// <summary>
    ///     List all roles in the system
    /// </summary>
    [HttpGet]
    [Route("Roles")]
    public async Task<IActionResult> ListRole([FromQuery] SieveModel sieveModel)
    {
        var result = await _sender.Send(new ListRoleQuery());

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        return base.Ok(await result.Value.ToPaginatedListAsync(_sieveProcessor, sieveModel, _httpContextAccessor));
    }

    /// <summary>
    ///   [Coordinator]  Create account for Contributor
    /// </summary>
    [HttpPost]
    [Route("CreateContributorAccount")]
    [Authorize(Roles = "Coordinator")]
    public async Task<IActionResult> CreateContributorAccount([FromBody] CreateContributorAccountCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => base.StatusCode(201, value),
            Problem);
    }

    /// <summary>
    ///   [Manager]  Create account for Coordinator
    /// </summary>
    [HttpPost]
    [Route("CreateCoordinatorAccount")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> CreateCoordinatorAccount([FromBody] CreateCoordinatorAccountCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => base.StatusCode(201, value),
            Problem);
    }

    /// <summary>
    ///   [Admin]  Admin can create account with all roles in the system    
    /// </summary>
    [HttpPost]
    [Route("CreateAllAccount")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAllAccount([FromBody] CreateAllAccountCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => base.StatusCode(201, value),
            Problem);
    }

    /// <summary>
    ///    For changing initial password of new user
    /// </summary>
    [HttpPost]
    [Route("ChangeInitialPassword")]
    public async Task<IActionResult> ChangeInitialPassword([FromBody] ChangeInitialPasswordCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => base.Ok(value),
            Problem);
    }



    /// <summary>
    ///   [Admin]  Change role of user
    /// </summary>
    [HttpPut]
    [Route("ChangeRole")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeRole([FromBody] AssignRoleCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => base.Ok(value),
            Problem);
    }


    /// <summary>
    ///   [Admin]  Change faculty of user
    /// </summary>
    [HttpPut]
    [Route("ChangeFaculty")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeFaculty([FromBody] ChangeFacultyCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => base.Ok(value),
            Problem);
    }

    /// <summary>
    ///   [Admin]  Edit user role and faculty
    /// </summary>
    [HttpPut]
    [Route("EditUser")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditUser([FromBody] EditUserCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => base.Ok(value),
            Problem);
    }

    /// <summary>
    ///   [Admin]  Toggle the active status of user
    /// </summary>
    [HttpPut]
    [Route("ToggleActive")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleActive([FromBody] ToggleActiveCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => base.Ok(value),
            Problem);
    }

}