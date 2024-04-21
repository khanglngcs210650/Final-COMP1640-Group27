namespace Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(
        Guid id,
        Guid? facultyId,
        string email,
        string firstName,
        string lastName,
        List<string> roles);
}