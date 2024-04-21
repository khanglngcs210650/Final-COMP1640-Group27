using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public bool IsInitialPasswordChanged { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public ICollection<Contribution> Contributions { get; set; }

    public ICollection<Feedback> Feedbacks { get; set; }

    public Guid? FacultyId { get; set; }

    public Faculty? Faculty { get; set; }

    public ICollection<ApplicationRole> Roles { get; set; }

    public Guid? AvatarId { get; set; }

    public Media Avatar { get; set; }

}