using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Identity.Services;


public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _contextAccessor;

    public CurrentUserProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }


    public CurrentUser? GetCurrentUser()
    {
        if (!CheckIfCurrentUserExist()) return null; //

        var idClaim = GetSingleClaimValue("id");
        var id = idClaim != null && Guid.TryParse(idClaim, out var parsedId) ? parsedId : Guid.Empty;

        var facultyIdClaim = GetSingleClaimValue("facultyId");
        var facultyId = facultyIdClaim != null && Guid.TryParse(facultyIdClaim, out var parsedFacultyId) ? parsedFacultyId : Guid.Empty;

        var roles = GetClaimValues(ClaimTypes.Role);
        var firstName = GetSingleClaimValue(ClaimTypes.GivenName) ?? string.Empty;
        var lastName = GetSingleClaimValue(ClaimTypes.Surname) ?? string.Empty;
        var email = GetSingleClaimValue(ClaimTypes.Email) ?? string.Empty;

        return new CurrentUser(id, facultyId, firstName, lastName, email, roles);
    }

    public bool CheckIfCurrentUserExist()
    {
        return _contextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    private List<string> GetClaimValues(string claimType) =>
        _contextAccessor.HttpContext?.User?.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList() ?? new List<string>();

    private string? GetSingleClaimValue(string claimType)
    {
        var claim = _contextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type == claimType);
        return claim?.Value;
    }
}