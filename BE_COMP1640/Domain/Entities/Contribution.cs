using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Contribution : AuditableBaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public ContributionStatus Status { get; set; }

    public ICollection<Feedback> Feedbacks { get; set; } // One-to-many

    public ICollection<Comment> Comments { get; set; }

    public ICollection<Rating> Ratings { get; set; }

    public Guid? PeriodId { get; set; }

    public Period Period { get; set; } // One-to-one

    public Guid? ImageId { get; set; }

    public Media Image { get; set; }

    public Guid? DocumentId { get; set; }

    public Media Document { get; set; }

}