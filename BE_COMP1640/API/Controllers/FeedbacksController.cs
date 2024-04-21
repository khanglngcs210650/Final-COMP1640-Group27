using API.Sieve;
using Application.Features.Feedbacks.Commands.CreateFeedback;
using Application.Features.Feedbacks.Queries.ListFeedback;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using Sieve.Services;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FeedbacksController : ApiController
{
    private readonly ISender _sender;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FeedbacksController(ISender sender, ISieveProcessor sieveProcessor,
        IHttpContextAccessor httpContextAccessor)
    {
        _sender = sender;
        _sieveProcessor = sieveProcessor;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///  [Coordinator] Create a new Feedback for a contribution
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Coordinator")]
    public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => StatusCode(201, value),
            Problem);
    }

    /// <summary>
    ///  [Coordinator, Contributor, Manager] Get a list of feedbacks
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Contributor, Coordinator, Manager")]
    public async Task<IActionResult> ListFeedback([FromQuery] SieveModel sieveModel)
    {
        var result = await _sender.Send(new ListFeedbackQuery());


        if (result.IsError)
        {
            return Problem(result.Errors);
        }


        return base.Ok(await result.Value.ToPaginatedListAsync(_sieveProcessor, sieveModel, _httpContextAccessor));
    }
}