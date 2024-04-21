using API.Sieve;
using Application.Features.Comments.Commands.CreateComment;
using Application.Features.Comments.Queries.ListComment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using Sieve.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ApiController
    {
        private readonly ISender _sender;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentsController(ISender sender, ISieveProcessor sieveProcessor,
            IHttpContextAccessor httpContextAccessor)
        {
            _sender = sender;
            _sieveProcessor = sieveProcessor;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        ///  [Contributor] Create a new comment for a contribution
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Contributor")]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentCommand request)
        {
            var result = await _sender.Send(request);

            return result.Match(
                value => StatusCode(201, value),
                Problem);
        }

        /// <summary>
        ///  Get a list of comments
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ListComment([FromQuery] SieveModel sieveModel)
        {
            var result = await _sender.Send(new ListCommentQuery());


            if (result.IsError)
            {
                return Problem(result.Errors);
            }


            return base.Ok(await result.Value.ToPaginatedListAsync(_sieveProcessor, sieveModel, _httpContextAccessor));
        }
    }
}
