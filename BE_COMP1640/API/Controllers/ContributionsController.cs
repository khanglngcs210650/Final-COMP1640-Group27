using API.RequestModels.Contributions;
using API.Sieve;
using Application.Common.Interfaces;
using Application.Features.Contributions.Commands.ApproveContribution;
using Application.Features.Contributions.Commands.CreateContribution;
using Application.Features.Contributions.Commands.LoveContribution;
using Application.Features.Contributions.Commands.PublishContribution;
using Application.Features.Contributions.Commands.UpdateContribution;
using Application.Features.Contributions.Queries.DownloadContribution;
using Application.Features.Contributions.Queries.GetContribution;
using Application.Features.Contributions.Queries.ListContribution;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using Sieve.Services;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContributionsController : ApiController
{
    private readonly ISender _sender;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContributionsController(ISender sender, ISieveProcessor sieveProcessor,
        ICurrentUserProvider currentUserProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        _sender = sender;
        _sieveProcessor = sieveProcessor;
        _currentUserProvider = currentUserProvider;
        _httpContextAccessor = httpContextAccessor;
    }


    /// <summary>
    ///    [Contributor] Creating a new Contribution
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Contributor")]
    public async Task<IActionResult> CreateContribution([FromForm] CreateContributionCommand request)
    {
        var result = await _sender.Send(request);
        return result.Match(
            value => StatusCode(201, value),
            Problem);
    }

    /// <summary>
    ///   [Contributor] Update one Contribution by id
    /// </summary>
    [HttpPut]
    [Route("{id:guid}")]
    [Authorize(Roles = "Contributor")]
    public async Task<IActionResult> UpdateContribution([FromRoute] Guid id, [FromForm] UpdateContributionRequest request)
    {
        var command = new UpdateContributionCommand(id, request.Title, request.Description, request.ImageFile,
            request.DocumentFile);

        var result = await _sender.Send(command);

        return result.Match(
            value => base.Ok(value),
            Problem);
    }

    /// <summary>
    ///   [Contributor] Toggle contribution love status
    /// </summary>
    [HttpPut]
    [Route("ToggleLoveContribution")]
    [Authorize(Roles = "Contributor")]
    public async Task<IActionResult> ToggleLoveContribution([FromBody] LoveContributionCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => base.Ok(value),
            Problem);
    }



    /// <summary>
    ///    Get list of Contributions
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListContribution([FromQuery] SieveModel sieveModel)
    {
        var result = await _sender.Send(new ListContributionQuery());


        if (result.IsError)
        {
            return Problem(result.Errors);
        }


        return base.Ok(await result.Value.ToPaginatedListAsync(_sieveProcessor, sieveModel, _httpContextAccessor));
    }


    /// <summary>
    ///    Get one Contribution by id
    /// </summary>
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetContribution([FromRoute] Guid id)
    {
        var query = new GetContributionQuery(id);

        var result = await _sender.Send(query);

        return result.Match(
            value => base.Ok(value),
            Problem);
    }

    /// <summary>
    ///  [Coordinator] Approve/Reject a contribution by it Id
    /// </summary>
    [HttpPut]
    [Route("Approval")]
    [Authorize(Roles = "Coordinator")]
    public async Task<IActionResult> ApproveContribution(
        [FromBody] ApproveContributionCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => base.Ok(value),
            Problem);
    }

    /// <summary>
    ///  [Manager] Publish/Unpublished a contribution by it Id
    /// </summary>
    [HttpPut]
    [Route("Publishment")]
    [Authorize(Roles = "Manager, Coordinator")]
    public async Task<IActionResult> PublishContribution([FromBody] PublishContributionCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => base.Ok(value),
            Problem);
    }


    /// <summary>
    ///  [Manager] Download all contributions as zip file
    /// </summary>
    [HttpGet]
    [Route("ZipAllContributions")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> ZipAllContributions()
    {
        var result = await _sender.Send(new DownloadContributionQuery());
        if (result.IsError)
        {
            return Problem(result.Errors);
        }


        return File(result.Value, "application/zip", "AllContributions.zip");
    }

}