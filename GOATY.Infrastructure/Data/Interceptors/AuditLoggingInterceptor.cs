using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GOATY.Infrastructure.Data.Interceptors
{
    public sealed class AuditLoggingInterceptor: SaveChangesInterceptor
    {
        private readonly IUser _user;
        public AuditLoggingInterceptor(IUser user)
        {
            _user = user;
        }
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateState(eventData);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateState(eventData);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateState(DbContextEventData eventData)
        {
            if (eventData.Context is null)
                return;

            foreach (var entry in eventData.Context.ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry != null && (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasOwnedChanged()))
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.CreatedAtUtc = DateTimeOffset.UtcNow;
                        entry.Entity.CreatedBy = _user.Id;
                    }
                    entry.Entity.LastModifiedUtc = DateTimeOffset.UtcNow;
                    entry.Entity.LastModifiedBy = _user.Id;
                }
            }
        }
    }

    public static class Extensions
    {
        public  static bool HasOwnedChanged(this EntityEntry entry)
        {
            return entry.References.Any(
                    e => e.TargetEntry?.Metadata.IsOwned() == true &&
                         (e.TargetEntry.State == EntityState.Added ||
                          e.TargetEntry.State == EntityState.Modified));
        }
    }
}