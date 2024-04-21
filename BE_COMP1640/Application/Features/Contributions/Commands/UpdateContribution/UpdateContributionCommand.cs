using Application.Common.Models;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Contributions.Commands.UpdateContribution;

public record UpdateContributionCommand : IRequest<ErrorOr<SuccessResult>>
{
    public UpdateContributionCommand(Guid id, string? title, string? description, IFormFile? imageFile, IFormFile? documentFile)
    {
        Id = id;
        Title = title;
        Description = description;
        ImageFile = imageFile;
        DocumentFile = documentFile;
    }

    public Guid Id { get; set; }

    /// <example>Business Administration in Graphic design?</example>
    public string? Title { get; set; }

    /// <example>Just descrption</example>
    public string? Description { get; set; }

    public IFormFile? ImageFile { get; set; }

    public IFormFile? DocumentFile { get; set; }
}