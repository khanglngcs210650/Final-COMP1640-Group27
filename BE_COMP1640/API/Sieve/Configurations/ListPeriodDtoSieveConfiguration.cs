using Application.Features.Periods.Queries.ListPeriod;
using Sieve.Services;

namespace API.Sieve.Configurations
{
    public class ListPeriodDtoSieveConfiguration : ISieveConfiguration
    {
        public void Configure(SievePropertyMapper mapper)
        {
            mapper.Property<ListPeriodDto>(x => x.AcademicYear)
                .CanFilter()
                .CanSort();

            mapper.Property<ListPeriodDto>(x => x.FirstSubmissionDeadline)
                .CanFilter()
                .CanSort();

            mapper.Property<ListPeriodDto>(x => x.SecondSubmissionDeadline)
                .CanFilter()
                .CanSort();
        }
    }
}
