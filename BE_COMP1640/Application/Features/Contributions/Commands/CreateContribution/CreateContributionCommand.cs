using Application.Common.Models;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Contributions.Commands.CreateContribution;

public record CreateContributionCommand : IRequest<ErrorOr<SuccessResult>>
{
    /// <example>An investigation on React and React Native for building cross-platform applications</example>
    public string Title { get; set; }

    /// <example>This description maybe yayayayyaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa</example>
    public string Description { get; set; }

    public IFormFile ImageFile { get; set; }

    public IFormFile DocumentFile { get; set; }

    public Guid PeriodId { get; set; }


}