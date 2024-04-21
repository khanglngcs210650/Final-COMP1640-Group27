using Domain.Common;

namespace Domain.Entities;

public class Faculty : BaseEntity
{
    public string Name { get; set; }

    public ICollection<ApplicationUser> Members { get; set; }

}