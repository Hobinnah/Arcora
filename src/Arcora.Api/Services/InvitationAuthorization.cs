using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Services;

public static class InvitationAuthorization
{
    public static long GetUserID(ClaimsPrincipal user) =>
        long.TryParse(user.FindFirst("UserId")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id)
            ? id : 0;

    public static Task<bool> CanManageAsync(ArcoraDbContext db, Guid organizationID, long userID) =>
        db.OrganizationMembers.AsNoTracking().AnyAsync(member =>
            userID > 0 && organizationID != Guid.Empty
            && member.OrganizationID == organizationID && member.UserID == userID
            && member.Status != null && member.Status.ToUpper() == "ACTIVE" && member.DeactivatedAt == null
            && (member.IsPrimaryOwner || (member.RoleName != null && member.RoleName.Trim().ToUpper() == "FULL ACCESS")));

    public static async Task<bool> CanManageInvitationAsync(ArcoraDbContext db, Guid id, long userID)
    {
        var organizationID = await (from invitation in db.TenantInvitations
                                    join listing in db.Listings on invitation.ListingID equals listing.ListingID
                                    where invitation.TenantInvitationID == id
                                    select (Guid?)listing.OrganizationID).FirstOrDefaultAsync();
        return organizationID.HasValue && await CanManageAsync(db, organizationID.Value, userID);
    }
}
