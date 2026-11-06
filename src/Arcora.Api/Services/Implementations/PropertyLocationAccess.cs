using System.Security.Claims;
using Arcora.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Services.Implementations;

public sealed class PropertyLocationAccess(ArcoraDbContext db)
{
    public static long? UserID(ClaimsPrincipal user) =>
        long.TryParse(user.FindFirstValue("UserId") ?? user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    public async Task<Dictionary<Guid, Guid>> ResourceAddressesAsync(IReadOnlyCollection<Guid> listingIDs,
        IReadOnlyCollection<Guid> propertyIDs, IReadOnlyCollection<Guid> unitIDs, CancellationToken ct)
    {
        var resources = new Dictionary<Guid, Guid>();
        if (listingIDs.Count > 0)
            foreach (var item in await db.Listings.AsNoTracking().Where(listing => listingIDs.Contains(listing.ListingID) &&
                listing.RentalUnit != null && listing.RentalUnit.Property != null)
                .Select(listing => new { ID = listing.ListingID, listing.RentalUnit!.Property!.AddressID }).ToListAsync(ct))
                resources[item.ID] = item.AddressID;
        if (propertyIDs.Count > 0)
            foreach (var item in await db.Properties.AsNoTracking().Where(property => propertyIDs.Contains(property.PropertyID))
                .Select(property => new { ID = property.PropertyID, property.AddressID }).ToListAsync(ct))
                resources[item.ID] = item.AddressID;
        if (unitIDs.Count > 0)
            foreach (var item in await db.RentalUnits.AsNoTracking().Where(unit => unitIDs.Contains(unit.RentalUnitID) && unit.Property != null)
                .Select(unit => new { ID = unit.RentalUnitID, unit.Property!.AddressID }).ToListAsync(ct))
                resources[item.ID] = item.AddressID;
        return resources;
    }

    public Task<bool> IsActiveAndPaidAsync(Lease lease, CancellationToken cancellationToken)
    {
        if (!string.Equals(lease.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(false);
        var key = $"pi_firstcharge_lease_{lease.LeaseID:N}";
        return CompletedPayments().AnyAsync(intent =>
            intent.LeaseID == lease.LeaseID && intent.TenantID == lease.TenantID && intent.IdempotencyKey == key,
            cancellationToken);
    }

    public sealed record Permissions(HashSet<Guid> Addresses, HashSet<Guid> Resources);

    public async Task<HashSet<Guid>> AllowedAddressesAsync(ClaimsPrincipal user, IReadOnlyCollection<Guid> addressIDs,
        CancellationToken cancellationToken) =>
        (await PermissionsAsync(user, addressIDs, new Dictionary<Guid, Guid>(), cancellationToken)).Addresses;

    public async Task<Permissions> PermissionsAsync(ClaimsPrincipal user, IReadOnlyCollection<Guid> addressIDs,
        IReadOnlyDictionary<Guid, Guid> resources,
        CancellationToken cancellationToken)
    {
        if (user.IsInRole("Admin"))
            return new(addressIDs.ToHashSet(), resources.Keys.ToHashSet());
        var userID = UserID(user);
        if (userID == null || addressIDs.Count == 0)
            return new([], []);

        var organizations = db.OrganizationMembers.AsNoTracking().Where(member =>
            member.UserID == userID && member.Status == "ACTIVE" && member.DeactivatedAt == null).Select(member => member.OrganizationID);
        var owned = await db.Addresses.AsNoTracking().Where(address => addressIDs.Contains(address.AddressID) &&
            ((address.OrganizationID != null && organizations.Contains(address.OrganizationID.Value)) ||
                db.Properties.Any(property => property.AddressID == address.AddressID && organizations.Contains(property.OrganizationID))))
            .Select(address => address.AddressID).ToListAsync(cancellationToken);
        var permittedResources = resources.Where(pair => owned.Contains(pair.Value)).Select(pair => pair.Key).ToHashSet();

        var candidates = await db.Leases.AsNoTracking().Where(lease => lease.Status == "ACTIVE" &&
            db.Tenants.Any(tenant => tenant.TenantID == lease.TenantID && tenant.UserID == userID) &&
            lease.RentalUnit != null && lease.RentalUnit.Property != null &&
            addressIDs.Contains(lease.RentalUnit.Property.AddressID))
            .Select(lease => new { lease.LeaseID, lease.TenantID, lease.ListingID, lease.RentalUnitID,
                lease.RentalUnit!.PropertyID, lease.RentalUnit.Property!.AddressID })
            .ToListAsync(cancellationToken);
        if (candidates.Count != 0)
        {
            var leaseIDs = candidates.Select(lease => lease.LeaseID).ToArray();
            var paid = await CompletedPayments().Where(intent => intent.LeaseID != null &&
                leaseIDs.Contains(intent.LeaseID.Value))
                .Select(intent => new { intent.LeaseID, intent.TenantID, intent.IdempotencyKey }).ToListAsync(cancellationToken);
            foreach (var lease in candidates.Where(lease => paid.Any(intent => intent.LeaseID == lease.LeaseID &&
                intent.TenantID == lease.TenantID && intent.IdempotencyKey == $"pi_firstcharge_lease_{lease.LeaseID:N}")))
            {
                owned.Add(lease.AddressID);
                permittedResources.UnionWith(new[] { lease.ListingID, lease.RentalUnitID, lease.PropertyID });
            }
        }
        return new(owned.ToHashSet(), permittedResources);
    }

    private IQueryable<PaymentIntent> CompletedPayments() =>
        db.PaymentIntents.AsNoTracking().Where(intent => intent.Amount > 0 &&
            db.Payments.Any(payment => payment.PaymentIntentID == intent.PaymentIntentID &&
                payment.TenantID == intent.TenantID && payment.Status == "PAID" && payment.PaidAt != null &&
                payment.GrossAmount >= intent.Amount && payment.RefundedAmount < payment.GrossAmount));

    public Task<bool> CanManageOrganizationAsync(ClaimsPrincipal user, Guid? organizationID, CancellationToken ct) =>
        user.IsInRole("Admin") ? Task.FromResult(true) : db.OrganizationMembers.AnyAsync(member =>
            member.UserID == UserID(user) && member.OrganizationID == organizationID && member.Status == "ACTIVE" && member.DeactivatedAt == null, ct);

    public Task<bool> CanManageAddressAsync(ClaimsPrincipal user, Guid id, CancellationToken ct) =>
        user.IsInRole("Admin") ? Task.FromResult(true) : db.Addresses.AnyAsync(address => address.AddressID == id &&
            db.OrganizationMembers.Any(member => member.UserID == UserID(user) && member.Status == "ACTIVE" && member.DeactivatedAt == null &&
                (address.OrganizationID == member.OrganizationID ||
                    db.Properties.Any(property => property.AddressID == address.AddressID && property.OrganizationID == member.OrganizationID))), ct);

    public Task<bool> CanManagePropertyAsync(ClaimsPrincipal user, Guid propertyID, CancellationToken ct) =>
        user.IsInRole("Admin") ? Task.FromResult(true) : db.Properties.AnyAsync(property => property.PropertyID == propertyID &&
            db.OrganizationMembers.Any(member => member.UserID == UserID(user) &&
                member.Status == "ACTIVE" && member.DeactivatedAt == null && member.OrganizationID == property.OrganizationID), ct);

    public Task<bool> HasHostMembershipAsync(ClaimsPrincipal user, CancellationToken ct) =>
        user.IsInRole("Admin") ? Task.FromResult(true) : db.OrganizationMembers.AnyAsync(member =>
            member.UserID == UserID(user) && member.Status == "ACTIVE" && member.DeactivatedAt == null, ct);
}
