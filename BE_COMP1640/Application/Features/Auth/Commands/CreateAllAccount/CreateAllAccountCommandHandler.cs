using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using ErrorOr;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Commands.CreateAllAccount
{
    public class CreateAllAccountCommandHandler : IRequestHandler<CreateAllAccountCommand, ErrorOr<SuccessResult>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IEmailService _emailService;

        public CreateAllAccountCommandHandler(
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

        public async Task<ErrorOr<SuccessResult>> Handle(CreateAllAccountCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _currentUserProvider.GetCurrentUser();

            if (currentUser == null) return Error.Unauthorized(description: "You are not authorized to this resource");

            //Check if email exist
            var emailExist = await _userManager.Users.AnyAsync(u => u.NormalizedEmail == request.Email.ToUpperInvariant(), cancellationToken);

            if (emailExist) return Error.Conflict(description: "This email already exists");

            var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());

            if (role == null) return Error.NotFound(description: "Role with the given RoleId not found");

            if ((role.Name == "Manager" || role.Name == "Admin") && request.FacultyId != null) return Error.Validation("You can't specify the faculty for Manager or Admin roles.");

            if ((role.Name == "Contributor" || role.Name == "Coordinator") && request.FacultyId == null) return Error.Validation("You must specify the faculty for Contributor or Coordinator roles.");


            var newUser = new ApplicationUser()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email
            };


            string? facultyName = null;

            if (request.FacultyId != null)
            {
                var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Id == request.FacultyId, cancellationToken);

                if (faculty == null) return Error.NotFound(description: "Faculty with the given FacultyId not found");

                newUser.FacultyId = request.FacultyId;

                facultyName = faculty.Name;
            }

            var randomPassword = GenerateRandomPassword();

            //Create new user
            var result = await _userManager.CreateAsync(newUser, randomPassword);

            if (!result.Succeeded) return Error.Unexpected(description: "Something went wrong, please try again");

            newUser.EmailConfirmed = true;

            //Add "Contributor" as default role for new contributor
            await _userManager.AddToRoleAsync(newUser, role.Name);

            // Compose email message
            var emailSubject = "ManagazineHub Contributor Account Details";

            var emailBody = $"Welcome to MagazineHub, our new {role.Name} <strong>{newUser.FirstName} {newUser.LastName}</strong>,<br/><br/>" +
                            $"Your account has been successfully granted by the Admin of MagazineHub<br/><br/>" +
                            $"Email/Username: {newUser.Email}<br/>" +
                            $"Password: <strong>{randomPassword}</strong><br/>" +
                            $"Faculty Name: <strong>{facultyName}</strong><br/><br/>" +
                            $"You can use this to log in to the system, you can reset your password after that";

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
