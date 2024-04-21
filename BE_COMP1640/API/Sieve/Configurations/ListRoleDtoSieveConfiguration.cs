using Application.Features.Auth.Queries.ListRole;
using Sieve.Services;

namespace API.Sieve.Configurations
{
    public class ListRoleDtoSieveConfiguration : ISieveConfiguration
    {
        public void Configure(SievePropertyMapper mapper)
        {
            mapper.Property<ListRoleDto>(x => x.Name)
                .CanFilter()
                .CanSort();
        }
    }
}
