using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PeriodConfiguration : IEntityTypeConfiguration<Period>
{
    public PeriodConfiguration() { }

    public void Configure(EntityTypeBuilder<Period> builder)
    {
        builder.Property(sd => sd.AcademicYear)
            .IsRequired();

        builder.Property(sd => sd.FirstSubmissionDeadline)
            .IsRequired();

        builder.Property(sd => sd.SecondSubmissionDeadline)
            .IsRequired();

        builder.HasOne<ApplicationUser>(s => s.CreatedBy)
            .WithMany()
            .HasForeignKey(s => s.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);
    }
}