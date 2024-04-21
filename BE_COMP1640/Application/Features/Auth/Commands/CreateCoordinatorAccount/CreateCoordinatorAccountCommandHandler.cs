using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using ErrorOr;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Commands.CreateCoordinatorAccount
{
    public class CreateCoordinatorAccountCommandHandler : IRequestHandler<CreateCoordinatorAccountCommand, ErrorOr<SuccessResult>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public CreateCoordinatorAccountCommandHandler(
            UserManager<ApplicationUser> userManager,
            IApplicationDbContext context,
            ICurrentUserProvider currentUserProvider,
            IEmailService emailService,
            IMapper mapper
        )
        {
            _userManager = userManager;
            _context = context;
            _currentUserProvider = currentUserProvider;
            _emailService = emailService;
            _mapper = mapper;
        }
        public async Task<ErrorOr<SuccessResult>> Handle(CreateCoordinatorAccountCommand request, CancellationToken cancellationToken)
        {
            //Check if email exist
            var emailExist = await _userManager.Users.AnyAsync(u => u.NormalizedEmail == request.Email.ToUpperInvariant(), cancellationToken);

            if (emailExist) return Error.Conflict(description: "This email already exists");

            var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Id == request.FacultyId, cancellationToken);

            if (faculty == null) return Error.NotFound(description: "Faculty not found");

            //Mapping 
            var newUser = _mapper.Map<ApplicationUser>(request);

            var randomPassword = GenerateRandomPassword();

            //Create new user
            var result = await _userManager.CreateAsync(newUser, randomPassword);

            if (!result.Succeeded) return Error.Unexpected(description: "Something went wrong, please try register again");

            newUser.EmailConfirmed = true;

            //Add "Contributor" as default role for new coordinator
            await _userManager.AddToRoleAsync(newUser, "Coordinator");

            // Compose email message
            var emailSubject = "ManagazineHub Coordinator Account Details";

            var emailBody = $"Welcome to MagazineHub, <strong>{newUser.FirstName} {newUser.LastName}</strong>, our new Coordinator<br/><br/>" +
                            $"Your account has been successfully granted by the Manager of MagazineHub<br/>" +
                            $"From today, you will be responsible for coordinating all activities related to contributions in the faculty of <strong>{faculty.Name}</strong><br/><br/>" +
                            $"Email/Username: {newUser.Email}<br/>" +
                            $"Password: <strong>{randomPassword}</strong><br/>" +
                            $"Faculty Name: <strong>{faculty.Name}</strong><br/><br/>" +
                            $"You can use above information to log in to the system, you can reset your password after that ";

            // Send email
            BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(newUser.Email, emailSubject, emailBody));

            return new SuccessResult(title: "User created successfully.");

        }

        private string GenerateRandomPassword()
        {
            var options = new PasswordOptions
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true,
                RequiredLength = 12
            };

            const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+=-";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, options.RequiredLength)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return password;
        }
    }
}
