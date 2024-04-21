using API.Sieve;
using Application.Features.Faculties.Commands.CreateFaculty;
using Application.Features.Faculties.Commands.EditFaculty;
using Application.Features.Faculties.Queries.ListFaculty;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using Sieve.Services;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FalcutiesController : ApiController
{
    private readonly ISender _sender;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FalcutiesController(ISender sender, ISieveProcessor sieveProcessor, IHttpContextAccessor httpContextAccessor)
    {
        _sender = sender;
        _sieveProcessor = sieveProcessor;
        _httpContextAccessor = httpContextAccessor;
    }


    /// <summary>
    ///   [Admin] Create a new Faculty
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateFaculty([FromBody] CreateFacultyCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => StatusCode(201, value),
            Problem);
    }


    /// <summary>
    ///    [Admin] Update a Faculty
    /// </summary>
    [HttpPut]
    [Route("UpdateFaculty")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditFaculty([FromBody] UpdateFacultyCommand request)
    {
        var result = await _sender.Send(request);

        return result.Match(
            value => StatusCode(200, value),
            Problem);
    }

    /// <summary>
    ///    Get list of Faculties
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListFaculties([FromQuery] SieveModel sieveModel)
    {
        var result = await _sender.Send(new ListFacultyQuery());


        if (result.IsError)
        {
            return Problem(result.Errors);
        }


        return base.Ok(await result.Value.ToPaginatedListAsync(_sieveProcessor, sieveModel, _httpContextAccessor));
    }

}