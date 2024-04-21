using FluentValidation;

namespace Application.Features.Feedbacks.Commands.CreateFeedback;

public class CreateFeedbackCommandValidator : AbstractValidator<CreateFeedbackCommand>
{
    public CreateFeedbackCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content must be provided")
            .MaximumLength(1000).WithMessage("Content must not exceed 1000 characters.");

        RuleFor(x => x.ContributionId)
            .NotEmpty().WithMessage("Contribution Id must be provided");
    }
}