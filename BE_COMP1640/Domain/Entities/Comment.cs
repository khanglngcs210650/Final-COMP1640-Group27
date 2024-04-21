using Domain.Common;

namespace Domain.Entities
{
    public class Comment : AuditableBaseEntity
    {
        public string Content { get; set; }

        public Guid ContributionId { get; set; } // Allow null for comments not associated with a contribution

        public Contribution Contribution { get; set; } // One-to-one (optional)

    }
}
