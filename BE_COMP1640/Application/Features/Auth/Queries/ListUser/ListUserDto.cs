namespace Application.Features.Auth.Queries.ListUser;

public class ListUserDto
{
    public Guid Id { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public string Role { get; set; }

    public string FacultyName { get; set; }

    public Guid? FacultyId { get; set; }

    public string AvatarUrl { get; set; }

    public bool IsActive { get; set; }
}