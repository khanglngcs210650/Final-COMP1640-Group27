using Domain.Common;

namespace Domain.Entities
{
    public class Rating : AuditableBaseEntity
    {
        public bool Loved { get; set; }

        public Guid ContributionId { get; set; } // Allow null for comments not associated with a contribution

        public Contribution Contribution { get; set; } // One-to-one (optional)
    }
}
