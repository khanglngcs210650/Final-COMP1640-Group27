using FluentValidation;

namespace Application.Features.Comments.Commands.CreateComment
{
    public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
    {
        public CreateCommentCommandValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content must be provided")
                .MaximumLength(250).WithMessage("Content must not exceed 1000 characters.");

            RuleFor(x => x.ContributionId)
                .NotEmpty().WithMessage("Contribution Id must be provided");
        }
    }
}
