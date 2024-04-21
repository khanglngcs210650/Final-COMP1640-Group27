using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ContributionConfiguration : IEntityTypeConfiguration<Contribution>
{
    public void Configure(EntityTypeBuilder<Contribution> builder)
    {

        builder.Property(c => c.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.HasMany<Feedback>(c => c.Feedbacks)
            .WithOne(f => f.Contribution)
            .HasForeignKey(f => f.ContributionId)
            .OnDelete(DeleteBehavior.NoAction);


        builder.HasMany<Comment>(c => c.Comments)
            .WithOne(c => c.Contribution)
            .HasForeignKey(c => c.ContributionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany<Rating>(c => c.Ratings)
            .WithOne(c => c.Contribution)
            .HasForeignKey(c => c.ContributionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<ApplicationUser>(c => c.CreatedBy)
            .WithMany(u => u.Contributions)
            .HasForeignKey(c => c.CreatedById)
            .HasPrincipalKey(u => u.Id)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Period>(c => c.Period)
            .WithMany(s => s.Contributions)
            .HasForeignKey(s => s.PeriodId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.Image)
            .WithOne()
            .HasForeignKey<Contribution>(c => c.ImageId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.Document)
            .WithOne()
            .HasForeignKey<Contribution>(c => c.DocumentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}