using Application.Features.Comments.Queries.ListComment;
using Sieve.Services;

namespace API.Sieve.Configurations
{
    public class ListCommentDtoSieveConfiguration : ISieveConfiguration
    {
        public void Configure(SievePropertyMapper mapper)
        {
            mapper.Property<ListCommentDto>(c => c.Content)
                .CanFilter()
                .CanSort();

            mapper.Property<ListCommentDto>(c => c.CreatedByEmail)
                .CanFilter()
                .CanSort();

            mapper.Property<ListCommentDto>(c => c.CreatedAt)
                .CanFilter()
                .CanSort();

            mapper.Property<ListCommentDto>(c => c.LastModifiedAt)
                .CanFilter()
                .CanSort();

            mapper.Property<ListCommentDto>(c => c.ContributionId)
                .CanFilter()
                .CanSort();
        }
    }
}
