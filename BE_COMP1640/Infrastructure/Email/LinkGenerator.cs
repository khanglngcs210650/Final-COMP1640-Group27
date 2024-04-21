using Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Email;

public class LinkGenerator : ILinkGenerator
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LinkGenerator(IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor)
    {
        _hostEnvironment = hostEnvironment;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GenerateLink(string url)
    {
        return $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/{url}";
    }

}