using API.RequestModels.Periods;
using API.Sieve;
using Application.Features.Periods.Commands.CreatePeriod;
using Application.Features.Periods.Commands.UpdatePeriod;
using Application.Features.Periods.Queries.ListPeriod;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using Sieve.Services;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PeriodsController : ApiController
{
    private readonly ISender _sender;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PeriodsController(ISender sender, ISieveProcessor sieveProcessor, IHttpContextAccessor httpContextAccessor)
    {
        _sender = sender;
        _sieveProcessor = sieveProcessor;
        _httpContextAccessor = httpContextAccessor;
    }


    /// <summary>
    ///   [Admin] Create a new Period
    /// </summary>
    [HttpPost]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePeriod([FromBody] CreatePeriodCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => StatusCode(201, value),
            Problem);
    }


    /// <summary>
    ///   [Admin] Update a Period by id
    /// </summary>
    [HttpPut]
    [Route("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePeriod([FromRoute] Guid id, [FromBody] UpdatePeriodRequest request)
    {
        var command = new UpdatePeriodCommand(id: id,
            firstSubmissionDeadline: request.FirstSubmissionDeadline,
            secondSubmissionDeadline: request.SecondSubmissionDeadline);

        var result = await _sender.Send(command);

        return result.Match(
            value => base.Ok(value),
            Problem);
    }

    /// <summary>
    ///    Get a list of periods
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListPeriod([FromQuery] SieveModel sieveModel)
    {
        var result = await _sender.Send(new ListPeriodQuery());

        if (result.IsError)
        {
            return Problem(result.Errors);
        }


        return base.Ok(await result.Value.ToPaginatedListAsync(_sieveProcessor, sieveModel, _httpContextAccessor));
    }

}