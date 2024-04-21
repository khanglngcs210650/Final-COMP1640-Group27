using Application.Features.Faculties.Queries.ListFaculty;
using Sieve.Services;

namespace API.Sieve.Configurations;

public class ListFacultyDtoSieveConfiguration : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<ListFacultyDto>(c => c.Name)
            .CanFilter()
            .CanSort();

        mapper.Property<ListFacultyDto>(c => c.CreatedAt)
            .CanFilter()
            .CanSort();

        mapper.Property<ListFacultyDto>(c => c.LastModifiedAt)
            .CanFilter()
            .CanSort();



    }
}