using Arcora.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Signing;

public static class LeaseSigningCompletion
{
    public static async Task<bool> ReconcileAsync(ArcoraDbContext dbContext, Lease lease,
        DateTime signedAt, string updatedBy, CancellationToken cancellationToken = default)
    {
        if (lease.Status is not ("PENDING_TENANT_SIGNATURE" or "PENDING_LANDLORD_SIGNATURE"
            or "SENT_FOR_SIGNATURE" or "PARTIALLY_SIGNED" or "PENDING_FIRST_PAYMENT"))
            return false;

        lease.SignedAt ??= signedAt;
        var firstChargeKey = $"pi_firstcharge_lease_{lease.LeaseID:N}";
        var firstPaymentPaid = await dbContext.PaymentIntents.AsNoTracking().AnyAsync(x =>
            x.LeaseID == lease.LeaseID && x.IdempotencyKey == firstChargeKey && x.Status == "PAID",
            cancellationToken);
        lease.Status = firstPaymentPaid ? "ACTIVE" : "PENDING_FIRST_PAYMENT";
        if (firstPaymentPaid) lease.ActivatedAt ??= DateTime.UtcNow;
        lease.UpdatedDate = DateTime.UtcNow;
        lease.UpdatedBy = updatedBy;
        return !firstPaymentPaid;
    }
}
