using Application.Common.Models;
using ErrorOr;
using MediatR;

namespace Application.Features.Auth.Commands.CreateContributorAccount
{
    public class CreateContributorAccountCommand : IRequest<ErrorOr<SuccessResult>>
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}
