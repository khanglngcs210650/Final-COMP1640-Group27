using Application.Features.Feedbacks.Queries.ListFeedback;
using Sieve.Services;

namespace API.Sieve.Configurations;

public class ListFeedbackDtoSieveConfiguration : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<ListFeedbackDto>(c => c.Content)
            .CanFilter()
            .CanSort();

        mapper.Property<ListFeedbackDto>(c => c.CreatedByEmail)
            .CanFilter()
            .CanSort();

        mapper.Property<ListFeedbackDto>(c => c.CreatedAt)
            .CanFilter()
            .CanSort();

        mapper.Property<ListFeedbackDto>(c => c.LastModifiedAt)
            .CanFilter()
            .CanSort();

        mapper.Property<ListFeedbackDto>(c => c.ContributionId)
            .CanFilter()
            .CanSort();
    }
}