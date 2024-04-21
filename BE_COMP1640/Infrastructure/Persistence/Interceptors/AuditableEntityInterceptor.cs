using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly TimeProvider _dateTime;
    private CurrentUser? CurrentUser => _currentUserProvider.GetCurrentUser();

    public AuditableEntityInterceptor(
        ICurrentUserProvider currentUserProvider,
        TimeProvider dateTime)
    {
        _currentUserProvider = currentUserProvider;
        _dateTime = dateTime;
    }


    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                var utcNow = _dateTime.GetUtcNow();
                if (entry.State == EntityState.Added)
                {
                    var createdByIdProperty = entry.Entity.GetType().GetProperty("CreatedById");
                    if (createdByIdProperty != null)
                    {
                        // Lấy giá trị của thuộc tính "CreatedById"
                        var createdByIdValue = createdByIdProperty.GetValue(entry.Entity);

                        // Nếu giá trị là null hoặc một giá trị mặc định, gán giá trị mới
                        if (createdByIdValue == null || createdByIdValue.Equals(default(Guid)))
                        {
                            // Gán giá trị mới từ CurrentUser?.Id
                            createdByIdProperty.SetValue(entry.Entity, CurrentUser?.Id);
                        }
                    }
                }
                if (entry.Entity.CreatedAt == default)
                {
                    entry.Entity.CreatedAt = utcNow;
                }

                if (entry.Entity.LastModifiedAt == default)
                {
                    entry.Entity.LastModifiedAt = utcNow;
                }
            }
        }
    }
}
public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}