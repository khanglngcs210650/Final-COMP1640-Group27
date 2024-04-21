using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        //Ensuring the database created
        await context.Database.EnsureCreatedAsync();

        //Migrating pending migration
        //if ((await context.Database.GetPendingMigrationsAsync()).Any())
        //{
        //    await context.Database.MigrateAsync();
        //}


        // Check if roles table have any data
        if (!context.Roles.Any())
        {
            // Seeding Roles
            var roles = new List<string> { "Admin", "Manager", "Coordinator", "Contributor" };
            foreach (var roleName in roles)
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }

            await context.SaveChangesAsync();
        }

        // Check if users table have any data
        if (!context.Users.Any())
        {
            // Create Admin account
            var admin = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                FirstName = "Admin",
                LastName = "Super",
                EmailConfirmed = true,
                IsInitialPasswordChanged = true
            };

            // Create Manager account
            var manager = new ApplicationUser
            {
                UserName = "manager@gmail.com",
                Email = "manager@gmail.com",
                FirstName = "Manager",
                LastName = "Super",
                EmailConfirmed = true,
                IsInitialPasswordChanged = true
            };

            await userManager.CreateAsync(admin, "Abcd@1234");
            await userManager.AddToRoleAsync(admin, "Admin");

            await userManager.CreateAsync(manager, "Abcd@1234");
            await userManager.AddToRoleAsync(manager, "Manager");

            await context.SaveChangesAsync();
        }

        // Check if faculties table have any data
        if (!context.Faculties.Any())
        {
            // Get admin user
            var admin = await userManager.FindByEmailAsync("admin@gmail.com");

            if (admin != null)
            {
                // Seeding Faculties
                var faculties = new List<string> { "Graphic Design", "IT", "Business", "Medical" };
                foreach (var facultyName in faculties)
                {
                    context.Faculties.Add(new Faculty
                    {
                        Name = facultyName
                    });
                }
                await context.SaveChangesAsync();
            }

            // Create 4 accounts for 4 coordinators of each faculty
            var coordinatorsToSeed = new List<(string facultyName, string emailPrefix)>
                {
                    ("IT", "it"),
                    ("Business", "business"),
                    ("Medical", "medical"),
                    ("Graphic Design", "graphicdesign"),
                };

            foreach (var (facultyName, emailPrefix) in coordinatorsToSeed)
            {
                var coordinator = new ApplicationUser
                {
                    UserName = $"{emailPrefix}@gmail.com",
                    Email = $"{emailPrefix}@gmail.com",
                    FirstName = $"{facultyName}",
                    LastName = "Coordinator",
                    FacultyId = context.Faculties.FirstOrDefault(f => f.Name == facultyName)?.Id,
                    EmailConfirmed = true,
                    IsInitialPasswordChanged = true
                };

                await userManager.CreateAsync(coordinator, "Abcd@1234");
                await userManager.AddToRoleAsync(coordinator, "Coordinator");

            }

            //Create 4 contributors for each faculty
            var contributorsToSeed = new List<(string facultyName, string emailPrefix, string firstName)>
                {
                    ("Graphic Design", "contributorGD", "Alice"),
                    ("IT", "contributorIT1", "Bob"),
                    ("IT", "contributorIT2", "Trung"),
                    ("Business", "contributorBUSI1", "Carol"),
                    ("Business", "contributorBUSI2", "PhuongAnh"),
                    ("Medical", "contributorMEDIC1", "David"),
                    ("Medical", "contributorMEDIC2", "David")
                };

            foreach (var (facultyName, emailPrefix, firstName) in contributorsToSeed)
            {
                var contributor = new ApplicationUser
                {
                    UserName = $"{emailPrefix}@gmail.com",
                    Email = $"{emailPrefix}@gmail.com",
                    FirstName = firstName,
                    LastName = "Contributor",
                    FacultyId = context.Faculties.FirstOrDefault(f => f.Name == facultyName)?.Id,
                    EmailConfirmed = true,
                    IsInitialPasswordChanged = true
                };

                await userManager.CreateAsync(contributor, "Abcd@1234");
                await userManager.AddToRoleAsync(contributor, "Contributor");
            }

            await context.SaveChangesAsync();
        }
        if (!context.Periods.Any())
        {
            var periods = new List<Period>();

            // Seed 3 periods for the years 2024, 2025, and 2026
            for (int year = 2024; year <= 2026; year++)
            {
                var period = new Period
                {
                    AcademicYear = year,
                    FirstSubmissionDeadline = new DateTime(year, 6, 30), // Assuming the first submission deadline is June 30th of each year
                    SecondSubmissionDeadline = new DateTime(year, 12, 31) // Assuming the second submission deadline is December 31st of each year
                };

                periods.Add(period);
            }

            context.Periods.AddRange(periods);

            // Seed contributions for each period
            foreach (var period in periods)
            {
                var newContributions = new List<Contribution>();

                var contributors = await userManager.GetUsersInRoleAsync("Contributor");

                foreach (var contributor in contributors)
                {
                    for (int i = 0; i < 3; i++) // Repeat 3 times for each status
                    {
                        var contributionNumber = i + 1;
                        foreach (ContributionStatus status in Enum.GetValues(typeof(ContributionStatus)))
                        {
                            DateTime createdAt;
                            createdAt = period.AcademicYear == DateTime.Now.Year ? DateTime.Now : new DateTime(period.AcademicYear, 5, 30);

                            //seed Document
                            var document = new Media
                            {
                                UrlFilePath = "https://www.w3.org/Protocols/HTTP/1.1/rfc2616.pdf"
                            };

                            //seed Image
                            var image = new Media
                            {
                                UrlFilePath = "https://dictionary.cambridge.org/vi/images/full/magazi_noun_002_22002.jpg?version=5.0.389"
                            };

                            var contribution = new Contribution
                            {
                                Title = $"Contribution {contributionNumber} by {contributor.FirstName} in {period.AcademicYear}",
                                Description = $"Description of Contribution {contributionNumber} by {contributor.FirstName} in {period.AcademicYear}",
                                Status = status,
                                CreatedById = contributor.Id,
                                PeriodId = period.Id,
                                Image = image,
                                Document = document,
                                CreatedAt = createdAt,
                                LastModifiedAt = createdAt
                            };
                            newContributions.Add(contribution);

                            //Seed feedbacks for non-processing contribution
                            if (status != ContributionStatus.Processing)
                            {
                                var newFeedbacks = new List<Feedback>();
                                var coordinatorsToFeedback = await userManager.GetUsersInRoleAsync("Coordinator");
                                var coordinator = coordinatorsToFeedback.FirstOrDefault(c => c.FacultyId == contributor.FacultyId);

                                for (int j = 0; j < 3; j++)
                                {
                                    var feedbackContent = $"Feedback {j + 1} for {contribution.Title}";
                                    var feedback = new Feedback
                                    {
                                        Content = feedbackContent,
                                        ContributionId = contribution.Id,
                                        CreatedById = coordinator?.Id
                                    };
                                    newFeedbacks.Add(feedback);
                                }
                                contribution.Feedbacks = newFeedbacks;
                            }
                        }
                    }
                }

                context.Contributions.AddRange(newContributions);
                await context.SaveChangesAsync();
            }
        }
    }
}