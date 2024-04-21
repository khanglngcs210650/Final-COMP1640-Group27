using Application.Features.Contributions.Queries.ListContribution;
using Sieve.Services;

namespace API.Sieve.Configurations;

public class ListContributionDtoSieveConfiguration : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<ListContributionDto>(c => c.Title)
            .CanFilter()
            .CanSort();

        mapper.Property<ListContributionDto>(c => c.Description)
            .CanFilter()
            .CanSort();

        mapper.Property<ListContributionDto>(c => c.Status)
            .CanFilter()
            .CanSort();

        mapper.Property<ListContributionDto>(c => c.FacultyName)
            .CanFilter()
            .CanSort();

        mapper.Property<ListContributionDto>(c => c.FacultyId)
            .CanFilter()
            .CanSort();

        mapper.Property<ListContributionDto>(c => c.PeriodId)
            .CanFilter()
            .CanSort();

        mapper.Property<ListContributionDto>(c => c.CreatedByEmail)
            .CanFilter()
            .CanSort();

        mapper.Property<ListContributionDto>(c => c.CreatedByFullName)
            .CanFilter()
            .CanSort();

        mapper.Property<ListContributionDto>(c => c.CreatedAt)
            .CanFilter()
            .CanSort();

        mapper.Property<ListContributionDto>(c => c.LastModifiedAt)
            .CanFilter()
            .CanSort();



    }
}