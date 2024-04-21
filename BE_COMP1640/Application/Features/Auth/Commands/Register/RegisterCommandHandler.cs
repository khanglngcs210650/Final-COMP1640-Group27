using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using ErrorOr;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Web;


namespace Application.Features.Auth.Commands.Register;



public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<SuccessResult>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly ILinkGenerator _linkGenerator;
    private readonly IApplicationDbContext _context;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IEmailService emailService,
        ILinkGenerator linkGenerator,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _mapper = mapper;
        _emailService = emailService;
        _linkGenerator = linkGenerator;
        _context = context;
    }
    public async Task<ErrorOr<SuccessResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        //Check if email and password exist
        var emailExist = await _userManager.Users.AnyAsync(u => u.NormalizedEmail == request.Email.ToUpperInvariant(), cancellationToken);

        if (emailExist) return Error.Conflict(description: "User already exists");

        var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Id == request.FacultyId, cancellationToken);

        if (faculty == null) return Error.NotFound(description: "Faculty not found");

        //Mapping 
        var newUser = _mapper.Map<ApplicationUser>(request);


        //Create new user
        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded) return Error.Unexpected(description: "Something went wrong, please try register again");



        //Add "Contributor" as default role for new user
        await _userManager.AddToRoleAsync(newUser, "Contributor");

        //Generate and sending confirming email
        var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

        confirmEmailToken = HttpUtility.UrlEncode(confirmEmailToken);

        var confirmationLink =
            _linkGenerator.GenerateLink(url: $"api/Auth/ConfirmEmail?token={confirmEmailToken}&email={newUser.Email}");

        BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(newUser.Email,
            "Email confirmation from MagazineHub, please click the link",
            $"Please click the following link to activate the account: {confirmationLink}"));

        return new SuccessResult(title: "Register successfully, please check your email for confirmation!");



    }
}