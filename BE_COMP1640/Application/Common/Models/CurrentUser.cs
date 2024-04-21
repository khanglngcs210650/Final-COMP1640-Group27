namespace Application.Common.Models;

public record CurrentUser(
    Guid Id,
    Guid FacultyId,
    string FirstName,
    string LastName,
    string Email,
    IReadOnlyList<string> Roles);