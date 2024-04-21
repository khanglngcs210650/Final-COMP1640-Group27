using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class FeedBackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.Property(f => f.Content)
            .HasMaxLength(1000)
            .IsRequired();

        builder.HasOne<ApplicationUser>(f => f.CreatedBy)
            .WithMany(u => u.Feedbacks)
            .HasForeignKey(f => f.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);



    }
}