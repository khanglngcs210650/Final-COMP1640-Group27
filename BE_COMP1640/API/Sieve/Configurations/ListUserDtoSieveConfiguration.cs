using Application.Features.Auth.Queries.ListUser;
using Sieve.Services;

namespace API.Sieve.Configurations;

public class ListUserDtoSieveConfiguration : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<ListUserDto>(c => c.FullName)
            .CanFilter()
            .CanSort();

        mapper.Property<ListUserDto>(c => c.Email)
            .CanFilter()
            .CanSort();

        mapper.Property<ListUserDto>(c => c.Role)
            .CanFilter()
            .CanSort();

        mapper.Property<ListUserDto>(c => c.FacultyName)
            .CanFilter()
            .CanSort();

        mapper.Property<ListUserDto>(c => c.FacultyId)
            .CanFilter()
            .CanSort();

        mapper.Property<ListUserDto>(c => c.IsActive)
            .CanFilter()
            .CanSort();

    }
}