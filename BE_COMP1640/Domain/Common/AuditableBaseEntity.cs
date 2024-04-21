using Domain.Entities;

namespace Domain.Common;

public abstract class AuditableBaseEntity : BaseEntity
{
    public Guid? CreatedById { get; set; }

    public ApplicationUser CreatedBy { get; set; }

    //public Guid LastModifiedById { get; set; }

    //public ApplicationUser LastModifiedBy { get; set; }
}