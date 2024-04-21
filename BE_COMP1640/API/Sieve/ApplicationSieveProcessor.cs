using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace API.Sieve;

public class ApplicationSieveProcessor : SieveProcessor

{
    protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
    {
        return mapper.ApplyConfigurationsFromAssembly(typeof(ApplicationSieveProcessor).Assembly);
    }

    public ApplicationSieveProcessor(IOptions<SieveOptions> options) : base(options)
    {
    }

    public ApplicationSieveProcessor(IOptions<SieveOptions> options, ISieveCustomSortMethods customSortMethods) : base(options, customSortMethods)
    {
    }

    public ApplicationSieveProcessor(IOptions<SieveOptions> options, ISieveCustomFilterMethods customFilterMethods) : base(options, customFilterMethods)
    {
    }

    public ApplicationSieveProcessor(IOptions<SieveOptions> options, ISieveCustomSortMethods customSortMethods, ISieveCustomFilterMethods customFilterMethods) : base(options, customSortMethods, customFilterMethods)
    {
    }
}