using System.Data;
using Arcora.Api.Entities;
using Arcora.Api.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Arcora.Api
{
    /// <summary>
    /// Global reservation concurrency guard. Every SaveChanges that adds a blocking reservation
    /// (hold, lease, calendar block) or expands one runs in a database transaction, takes a
    /// transaction-owned SQL Server application lock per listing (sorted), and validates overlaps
    /// against persisted and tracked blocking intervals before the base save.
    /// </summary>
    public partial class ArcoraDbContext
    {
        public const int ListingLockTimeoutMilliseconds = 15000;

        private const string SqlServerProvider = "Microsoft.EntityFrameworkCore.SqlServer";
        private const string SqliteProvider = "Microsoft.EntityFrameworkCore.Sqlite";

        // Kept in sync with ListingService availability status lists (upper-case).
        public static readonly string[] NonBlockingLeaseStatuses =
            { "CANCELLED", "CANCELED", "EXPIRED", "RELEASED", "TERMINATED", "DRAFT", "REJECTED", "VOID" };
        public static readonly string[] NonBlockingHoldStatuses =
            { "CANCELLED", "CANCELED", "EXPIRED", "RELEASED" };
        public static readonly string[] NonBlockingCalendarEventStatuses =
            { "CANCELLED", "CANCELED", "DELETED" };

        /// <summary>
        /// Acquires the exclusive reservation lock for a listing, owned by the current transaction.
        /// Must be called inside a transaction, before availability reads.
        /// </summary>
        public Task AcquireListingLockAsync(Guid listingID, CancellationToken cancellationToken = default)
            => AcquireListingLocksAsync(this, new[] { listingID }, cancellationToken);

        /// <summary>
        /// Static form of <see cref="AcquireListingLockAsync(Guid, CancellationToken)"/>.
        /// </summary>
        public static Task AcquireListingLockAsync(ArcoraDbContext dbContext, Guid listingID, CancellationToken cancellationToken = default)
            => AcquireListingLocksAsync(dbContext, new[] { listingID }, cancellationToken);

        /// <summary>
        /// Acquires reservation locks for several listings in a deterministic (sorted) order.
        /// </summary>
        public static async Task AcquireListingLocksAsync(ArcoraDbContext dbContext, IEnumerable<Guid> listingIDs, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            var ids = listingIDs.Where(id => id != Guid.Empty).Distinct().OrderBy(id => id).ToList();
            if (ids.Count == 0)
                return;

            if (dbContext.Database.CurrentTransaction == null && System.Transactions.Transaction.Current == null)
                throw new InvalidOperationException("A database transaction is required before acquiring a listing reservation lock.");

            var provider = dbContext.Database.ProviderName;
            if (provider == SqliteProvider)
            {
                // SQLite (tests only) has a single database-wide writer lock and no application locks;
                // the overlap validation still runs, which is sufficient for sequential tests.
                return;
            }

            if (provider != SqlServerProvider)
                throw new InvalidOperationException($"Listing reservation locks are not supported for database provider '{provider}'.");

            foreach (var id in ids)
            {
                var result = new SqlParameter("@result", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var resource = new SqlParameter("@resource", SqlDbType.NVarChar, 255) { Value = ListingLockResource(id) };
                var timeout = new SqlParameter("@timeout", SqlDbType.Int) { Value = ListingLockTimeoutMilliseconds };

                await dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC @result = sp_getapplock @Resource = @resource, @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = @timeout;",
                    new object[] { result, resource, timeout },
                    cancellationToken);

                var code = result.Value is int value ? value : -999;
                if (code >= 0)
                    continue;

                if (code is -1 or -2 or -3)
                {
                    throw new ApiProblemException(
                        StatusCodes.Status409Conflict,
                        "Listing busy",
                        "Another reservation change for this listing is in progress. Please retry.");
                }

                throw new InvalidOperationException($"Unable to acquire listing reservation lock (sp_getapplock returned {code}).");
            }
        }

        public static string ListingLockResource(Guid listingID) => $"arcora:listing-reservation:{listingID:N}";

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var candidates = CollectReservationCandidates();
            if (candidates.Count == 0)
                return base.SaveChanges(acceptAllChangesOnSuccess);

            IDbContextTransaction? ownedTransaction = BeginGuardTransactionIfNeeded();
            try
            {
                EnforceReservationGuardAsync(candidates, CancellationToken.None).GetAwaiter().GetResult();
                var result = base.SaveChanges(acceptAllChangesOnSuccess);
                ownedTransaction?.Commit();
                return result;
            }
            finally
            {
                ownedTransaction?.Dispose();
            }
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var candidates = CollectReservationCandidates();
            if (candidates.Count == 0)
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            IDbContextTransaction? ownedTransaction = Database.CurrentTransaction == null && System.Transactions.Transaction.Current == null
                ? await Database.BeginTransactionAsync(cancellationToken)
                : null;
            try
            {
                await EnforceReservationGuardAsync(candidates, cancellationToken);
                var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
                if (ownedTransaction != null)
                    await ownedTransaction.CommitAsync(cancellationToken);
                return result;
            }
            finally
            {
                if (ownedTransaction != null)
                    await ownedTransaction.DisposeAsync();
            }
        }

        private IDbContextTransaction? BeginGuardTransactionIfNeeded()
            => Database.CurrentTransaction == null && System.Transactions.Transaction.Current == null
                ? Database.BeginTransaction()
                : null;

        private async Task EnforceReservationGuardAsync(List<ReservationInterval> candidates, CancellationToken cancellationToken)
        {
            await AcquireListingLocksAsync(this, candidates.Select(c => c.ListingID), cancellationToken);

            var now = DateTime.UtcNow;
            var tracked = TrackedBlockingIntervals(now);
            var trackedKeys = ChangeTracker.Entries()
                .Where(e => e.Entity is Lease or ReservationHold or CalendarEvent)
                .Select(e => KeyOf(e.Entity))
                .ToHashSet();

            foreach (var group in candidates.GroupBy(c => c.ListingID))
            {
                var listingID = group.Key;
                var windowStart = group.Min(c => c.Start);
                var windowEnd = group.Max(c => c.End);

                var others = tracked.Where(t => t.ListingID == listingID).ToList();
                others.AddRange((await PersistedBlockingIntervalsAsync(listingID, windowStart, windowEnd, now, cancellationToken))
                    .Where(p => !trackedKeys.Contains((p.Kind, p.ID))));

                foreach (var candidate in group)
                {
                    var conflict = others.FirstOrDefault(o =>
                        !(o.Kind == candidate.Kind && o.ID == candidate.ID)
                        && candidate.Start < o.End && candidate.End > o.Start
                        && !IsSameReservation(candidate, o));

                    if (conflict != null)
                    {
                        throw new ApiProblemException(
                            StatusCodes.Status409Conflict,
                            "Listing unavailable",
                            $"The requested dates ({candidate.Start:yyyy-MM-dd} to {candidate.End:yyyy-MM-dd}) overlap an existing {FriendlyKind(conflict.Kind)} on this listing ({conflict.Start:yyyy-MM-dd} to {(conflict.End == DateTime.MaxValue ? "open-ended" : conflict.End.ToString("yyyy-MM-dd"))}).");
                    }
                }
            }
        }

        /// <summary>
        /// Added blocking reservations, or modified ones whose blocking footprint expanded
        /// (became blocking, moved listing, or widened dates). Innocuous updates are skipped.
        /// </summary>
        private List<ReservationInterval> CollectReservationCandidates()
        {
            if (ChangeTracker.AutoDetectChangesEnabled)
                ChangeTracker.DetectChanges();

            var now = DateTime.UtcNow;
            var candidates = new List<ReservationInterval>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State is not (EntityState.Added or EntityState.Modified))
                    continue;
                if (entry.Entity is not (Lease or ReservationHold or CalendarEvent))
                    continue;

                var current = Project(entry.Entity, entry.CurrentValues, now);
                if (current == null)
                    continue;

                if (entry.State == EntityState.Added)
                {
                    candidates.Add(current);
                    continue;
                }

                var original = Project(entry.Entity, entry.OriginalValues, now);
                if (original == null
                    || original.ListingID != current.ListingID
                    || current.Start < original.Start
                    || current.End > original.End)
                {
                    candidates.Add(current);
                }
            }

            return candidates;
        }

        private List<ReservationInterval> TrackedBlockingIntervals(DateTime now)
        {
            return ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Unchanged)
                .Where(e => e.Entity is Lease or ReservationHold or CalendarEvent)
                .Select(e => Project(e.Entity, e.CurrentValues, now))
                .Where(i => i != null)
                .Select(i => i!)
                .ToList();
        }

        private async Task<List<ReservationInterval>> PersistedBlockingIntervalsAsync(
            Guid listingID, DateTime windowStart, DateTime windowEnd, DateTime now, CancellationToken cancellationToken)
        {
            var leaseStatuses = NonBlockingLeaseStatuses;
            var holdStatuses = NonBlockingHoldStatuses;
            var eventStatuses = NonBlockingCalendarEventStatuses;

            var leases = await Leases.AsNoTracking()
                .Where(l => l.ListingID == listingID
                    && !leaseStatuses.Contains((l.Status ?? string.Empty).ToUpper())
                    && l.StartDate < windowEnd
                    && (l.EndDate ?? DateTime.MaxValue) > windowStart)
                .Select(l => new { l.LeaseID, l.StartDate, l.EndDate })
                .ToListAsync(cancellationToken);

            var holds = await ReservationHolds.AsNoTracking()
                .Where(h => h.ListingID == listingID
                    && !holdStatuses.Contains((h.Status ?? string.Empty).ToUpper())
                    && h.ReleasedAt == null
                    && h.ExpiresAt > now
                    && h.StartDate < windowEnd
                    && h.EndDate > windowStart)
                .Select(h => new { h.ReservationHoldID, h.StartDate, h.EndDate })
                .ToListAsync(cancellationToken);

            var events = await CalendarEvents.AsNoTracking()
                .Where(e => e.ListingID == listingID
                    && e.BlocksAvailability
                    && !eventStatuses.Contains((e.Status ?? string.Empty).ToUpper())
                    && e.StartAt < windowEnd
                    && e.EndAt > windowStart)
                .Select(e => new { e.CalendarEventID, e.StartAt, e.EndAt, e.LeaseID, e.ReservationHoldID })
                .ToListAsync(cancellationToken);

            return leases.Select(l => new ReservationInterval(ReservationKind.Lease, l.LeaseID, listingID, l.StartDate, l.EndDate ?? DateTime.MaxValue, null, null))
                .Concat(holds.Select(h => new ReservationInterval(ReservationKind.Hold, h.ReservationHoldID, listingID, h.StartDate, h.EndDate, null, null)))
                .Concat(events.Select(e => new ReservationInterval(ReservationKind.CalendarEvent, e.CalendarEventID, listingID, e.StartAt, e.EndAt, e.LeaseID, e.ReservationHoldID)))
                .ToList();
        }

        private static ReservationInterval? Project(object entity, PropertyValues values, DateTime now)
        {
            switch (entity)
            {
                case Lease:
                {
                    var status = values.GetValue<string?>(nameof(Lease.Status));
                    if (IsNonBlocking(status, NonBlockingLeaseStatuses))
                        return null;
                    return new ReservationInterval(
                        ReservationKind.Lease,
                        values.GetValue<Guid>(nameof(Lease.LeaseID)),
                        values.GetValue<Guid>(nameof(Lease.ListingID)),
                        values.GetValue<DateTime>(nameof(Lease.StartDate)),
                        values.GetValue<DateTime?>(nameof(Lease.EndDate)) ?? DateTime.MaxValue,
                        null, null);
                }
                case ReservationHold:
                {
                    var status = values.GetValue<string?>(nameof(ReservationHold.Status));
                    if (IsNonBlocking(status, NonBlockingHoldStatuses)
                        || values.GetValue<DateTime?>(nameof(ReservationHold.ReleasedAt)) != null
                        || values.GetValue<DateTime>(nameof(ReservationHold.ExpiresAt)) <= now)
                        return null;
                    return new ReservationInterval(
                        ReservationKind.Hold,
                        values.GetValue<Guid>(nameof(ReservationHold.ReservationHoldID)),
                        values.GetValue<Guid>(nameof(ReservationHold.ListingID)),
                        values.GetValue<DateTime>(nameof(ReservationHold.StartDate)),
                        values.GetValue<DateTime>(nameof(ReservationHold.EndDate)),
                        null, null);
                }
                case CalendarEvent:
                {
                    var status = values.GetValue<string?>(nameof(CalendarEvent.Status));
                    if (!values.GetValue<bool>(nameof(CalendarEvent.BlocksAvailability))
                        || IsNonBlocking(status, NonBlockingCalendarEventStatuses))
                        return null;
                    return new ReservationInterval(
                        ReservationKind.CalendarEvent,
                        values.GetValue<Guid>(nameof(CalendarEvent.CalendarEventID)),
                        values.GetValue<Guid>(nameof(CalendarEvent.ListingID)),
                        values.GetValue<DateTime>(nameof(CalendarEvent.StartAt)),
                        values.GetValue<DateTime>(nameof(CalendarEvent.EndAt)),
                        values.GetValue<Guid?>(nameof(CalendarEvent.LeaseID)),
                        values.GetValue<Guid?>(nameof(CalendarEvent.ReservationHoldID)));
                }
                default:
                    return null;
            }
        }

        private static bool IsNonBlocking(string? status, string[] nonBlocking)
            => nonBlocking.Contains((status ?? string.Empty).ToUpperInvariant());

        /// <summary>
        /// Calendar entries linked to a lease/hold (or to each other via the same lease/hold)
        /// describe the same reservation and never conflict with it.
        /// </summary>
        private static bool IsSameReservation(ReservationInterval a, ReservationInterval b)
        {
            static bool Linked(ReservationInterval evt, ReservationInterval other) =>
                evt.Kind == ReservationKind.CalendarEvent
                && ((other.Kind == ReservationKind.Lease && evt.LinkedLeaseID == other.ID)
                    || (other.Kind == ReservationKind.Hold && evt.LinkedHoldID == other.ID));

            if (Linked(a, b) || Linked(b, a))
                return true;

            return a.Kind == ReservationKind.CalendarEvent && b.Kind == ReservationKind.CalendarEvent
                && ((a.LinkedLeaseID != null && a.LinkedLeaseID == b.LinkedLeaseID)
                    || (a.LinkedHoldID != null && a.LinkedHoldID == b.LinkedHoldID));
        }

        private static (ReservationKind Kind, Guid ID) KeyOf(object entity) => entity switch
        {
            Lease l => (ReservationKind.Lease, l.LeaseID),
            ReservationHold h => (ReservationKind.Hold, h.ReservationHoldID),
            CalendarEvent e => (ReservationKind.CalendarEvent, e.CalendarEventID),
            _ => throw new ArgumentOutOfRangeException(nameof(entity))
        };

        private static string FriendlyKind(ReservationKind kind) => kind switch
        {
            ReservationKind.Lease => "lease",
            ReservationKind.Hold => "reservation hold",
            _ => "calendar block"
        };

        private enum ReservationKind
        {
            Lease,
            Hold,
            CalendarEvent
        }

        private sealed record ReservationInterval(
            ReservationKind Kind,
            Guid ID,
            Guid ListingID,
            DateTime Start,
            DateTime End,
            Guid? LinkedLeaseID,
            Guid? LinkedHoldID);
    }
}
