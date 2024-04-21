using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Contribution> Contributions => Set<Contribution>();

    public DbSet<Faculty> Faculties => Set<Faculty>();

    public DbSet<Feedback> Feedbacks => Set<Feedback>();

    public DbSet<Media> Media => Set<Media>();

    public DbSet<Period> Periods => Set<Period>();

    public DbSet<Comment> Comments => Set<Comment>();

    public DbSet<Rating> Ratings => Set<Rating>();

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

    public DbSet<ApplicationRole> Roles => Set<ApplicationRole>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity<IdentityUserRole<Guid>>(
                j => j.HasOne<ApplicationRole>().WithMany().HasForeignKey(ur => ur.RoleId),
                j => j.HasOne<ApplicationUser>().WithMany().HasForeignKey(ur => ur.UserId),
                j =>
                {
                    j.ToTable("UserRoles");
                });

        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
            // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
            // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
            // use the DateTimeOffsetToBinaryConverter
            // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
            // This only supports millisecond precision, but should be sufficient for most use cases.
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
                                                                               || p.PropertyType == typeof(DateTimeOffset?));
                foreach (var property in properties)
                {
                    builder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(new DateTimeOffsetToBinaryConverter());
                }
            }
        }

    }

}