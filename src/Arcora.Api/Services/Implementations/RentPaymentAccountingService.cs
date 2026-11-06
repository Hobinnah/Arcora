using System.Globalization;
using System.Data;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Services.Implementations;

public sealed class RentPaymentAccountingService(ArcoraDbContext db)
{
    public async Task ReconcileAsync(Guid paymentId, RentPaymentReconciliation expected, string actor, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(actor) || actor.Length > 256)
            throw new ArgumentException("An audit actor is required.", nameof(actor));
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var payment = await db.Payments.AsNoTracking().SingleAsync(x => x.PaymentID == paymentId, ct);
        if (db.Database.IsSqlServer())
            await db.PaymentIntents.FromSqlInterpolated(
                $"SELECT * FROM [PaymentIntents] WITH (UPDLOCK, HOLDLOCK) WHERE [PaymentIntentID] = {payment.PaymentIntentID}")
                .AsNoTracking().ToListAsync(ct);
        var intent = await db.PaymentIntents.AsNoTracking().SingleAsync(x => x.PaymentIntentID == payment.PaymentIntentID, ct);
        if (intent.Status != "PAID" || payment.Status != "PAID" || payment.RefundedAmount != 0m)
            throw new InvalidOperationException("Reconciliation requires a confirmed, unrefunded payment.");
        var allocation = await db.RentPaymentAccountings.SingleOrDefaultAsync(
            x => x.PaymentIntentID == intent.PaymentIntentID, ct);
        if (allocation == null)
        {
            allocation = await SnapshotAsync(intent, ct)
                ?? throw new InvalidOperationException("This payment has no supported rent collection key.");
            var funded = (await db.SecurityDepositTransactions.AsNoTracking()
                .Where(x => x.PaymentID == paymentId && x.TransactionType == "FUNDING")
                .Select(x => x.Amount).ToListAsync(ct)).Sum();
            if (funded != allocation.DepositAmount)
                throw new InvalidOperationException("The deposit allocation does not match the recorded deposit funding.");
            allocation.ReconciliationBy = actor;
            db.RentPaymentAccountings.Add(allocation);
        }
        if (expected != new RentPaymentReconciliation(allocation.RentAmount, allocation.DepositAmount,
            allocation.TenantFeeAmount, allocation.PlacementFeeRate, allocation.MonthlyFeeRate))
            throw new InvalidOperationException("The explicitly approved allocation does not match the payment snapshot.");
        await db.SaveChangesAsync(ct);
        await SettleAsync(payment, ct);
        await transaction.CommitAsync(ct);
    }

    public async Task<RentPaymentAccounting?> SnapshotAsync(PaymentIntent intent, CancellationToken ct)
    {
        if (!intent.LeaseID.HasValue)
            return null;
        var first = intent.IdempotencyKey?.StartsWith("pi_firstcharge_", StringComparison.Ordinal) == true;
        var recurringPrefix = $"pi_{intent.LeaseID:N}_";
        var recurring = intent.IdempotencyKey?.StartsWith(recurringPrefix, StringComparison.Ordinal) == true &&
            DateTime.TryParseExact(intent.IdempotencyKey[recurringPrefix.Length..], "yyyyMM",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        if (!first && !recurring)
            return null;

        var lease = await db.Leases.AsNoTracking().SingleAsync(x => x.LeaseID == intent.LeaseID, ct);
        var unresolved = from previousPayment in db.Payments.AsNoTracking()
                         join previousIntent in db.PaymentIntents.AsNoTracking()
                             on previousPayment.PaymentIntentID equals previousIntent.PaymentIntentID
                         join previousLease in db.Leases.AsNoTracking() on previousIntent.LeaseID equals previousLease.LeaseID
                         where previousLease.RentalUnitID == lease.RentalUnitID &&
                             previousIntent.PaymentIntentID != intent.PaymentIntentID &&
                             previousIntent.ScheduledChargeAt <= intent.ScheduledChargeAt &&
                             previousPayment.Status == "PAID" &&
                             !db.RentPaymentAccountings.Any(x =>
                                 x.PaymentID == previousPayment.PaymentID && x.SettledAt != null)
                         select previousPayment.PaymentID;
        if (await unresolved.AnyAsync(ct))
            throw new InvalidOperationException("Reconcile earlier paid rent for this unit before collecting or allocating another payment.");
        var deposit = first ? await db.SecurityDeposits.AsNoTracking()
            .Where(x => x.LeaseID == lease.LeaseID).Select(x => x.RequiredAmount).SingleOrDefaultAsync(ct) : 0m;
        var now = DateTime.UtcNow;
        var fees = await db.Fees.AsNoTracking().Include(x => x.FeeType)
            .Where(x => x.IsActive && (x.OrganizationID == null || x.OrganizationID == lease.OrganizationID) &&
                (x.EffectiveFrom == null || x.EffectiveFrom <= now) &&
                (x.EffectiveTo == null || x.EffectiveTo >= now)).ToListAsync(ct);
        var placement = HostFee(fees, "PLATFORM-HOST-PLACEMENT", "Host Placement Fee", lease);
        var monthly = HostFee(fees, "PLATFORM-HOST-UNIT-SUBSCRIPTION", "Host Active Unit Subscription Fee", lease);
        var tenantFees = first ? TenantBookingFees.Calculate(
            fees.Where(x => x.FeeType?.IsPlatformFee == true), lease.BaseRentAmount) : 0m;
        if (intent.Amount != lease.BaseRentAmount + deposit + tenantFees)
            throw new InvalidOperationException("The collection does not match the rent, deposit and tenant fee breakdown.");
        if (lease.BaseRentAmount <= 0m || deposit < 0m || placement + monthly > lease.BaseRentAmount)
            throw new InvalidOperationException("Host deductions must not exceed the rent collected.");
        var period = first ? lease.StartDate : DateTime.ParseExact(
            intent.IdempotencyKey![recurringPrefix.Length..], "yyyyMM", CultureInfo.InvariantCulture);
        return new RentPaymentAccounting
        {
            PaymentIntentID = intent.PaymentIntentID, LeaseID = lease.LeaseID,
            OrganizationID = lease.OrganizationID, ListingID = lease.ListingID,
            RentalUnitID = lease.RentalUnitID,
            RentPeriod = new DateTime(period.Year, period.Month, 1),
            Currency = intent.Currency ?? "CAD", RentAmount = lease.BaseRentAmount,
            DepositAmount = deposit, TenantFeeAmount = tenantFees,
            PlacementFeeRate = placement, MonthlyFeeRate = monthly, CapturedDate = now
        };
    }

    public async Task CreateInvoiceAsync(PaymentIntent intent, RentPaymentAccounting allocation, CancellationToken ct)
    {
        if (intent.InvoiceMasterID.HasValue)
            return;

        var lease = await db.Leases.AsNoTracking().SingleAsync(x => x.LeaseID == allocation.LeaseID, ct);
        var lines = new List<InvoiceDetail>
        {
            CreateInvoiceLine("RENT", $"Rent - {allocation.RentPeriod:yyyy-MM}",
                allocation.RentAmount, servicePeriodStart: allocation.RentPeriod,
                servicePeriodEnd: allocation.RentPeriod.AddMonths(1).AddDays(-1))
        };
        if (allocation.DepositAmount > 0m)
            lines.Add(CreateInvoiceLine("SECURITY_DEPOSIT", "Security deposit", allocation.DepositAmount));

        if (allocation.TenantFeeAmount > 0m)
        {
            var fees = await db.Fees.AsNoTracking().Include(x => x.FeeType)
                .Where(x => x.IsActive && x.FeeType != null && x.FeeType.IsPlatformFee == true &&
                    (x.OrganizationID == null || x.OrganizationID == lease.OrganizationID) &&
                    (x.EffectiveFrom == null || x.EffectiveFrom <= DateTime.UtcNow) &&
                    (x.EffectiveTo == null || x.EffectiveTo >= DateTime.UtcNow))
                .ToListAsync(ct);
            foreach (var fee in fees.Where(TenantBookingFees.IsTenantFee))
            {
                if (!string.Equals(fee.Currency, allocation.Currency, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"Fee {fee.FeeID} does not use the invoice currency.");
                var amount = TenantBookingFees.CalculateAmount(fee, allocation.RentAmount);
                if (amount <= 0m)
                    continue;
                var lineType = fee.Code?.Contains("BOOKING", StringComparison.OrdinalIgnoreCase) == true ||
                    fee.Name?.Contains("booking", StringComparison.OrdinalIgnoreCase) == true
                    ? "BOOKING_FEE"
                    : "PLATFORM_FEE";
                lines.Add(CreateInvoiceLine(lineType, fee.Name ?? "Platform fee", amount, fee.FeeID));
            }
        }

        if (lines.Sum(x => x.TotalLineAmount) != intent.Amount ||
            lines.Where(x => x.LineType is "BOOKING_FEE" or "PLATFORM_FEE").Sum(x => x.LineAmount) != allocation.TenantFeeAmount)
            throw new InvalidOperationException("The invoice line items do not match the amount being collected.");

        var now = DateTime.UtcNow;
        var invoice = new InvoiceMaster
        {
            InvoiceMasterID = Guid.NewGuid(),
            LeaseID = lease.LeaseID,
            OrganizationID = lease.OrganizationID,
            TenantID = intent.TenantID,
            InvoiceNumber = $"INV-{intent.PaymentIntentID:N}",
            BillingPeriodStart = allocation.RentPeriod,
            BillingPeriodEnd = allocation.RentPeriod.AddMonths(1).AddDays(-1),
            DueDate = intent.ScheduledChargeAt,
            SubtotalAmount = lines.Sum(x => x.LineAmount),
            TaxAmount = lines.Sum(x => x.TaxAmount),
            TotalAmount = intent.Amount,
            AmountPaid = 0m,
            BalanceDue = intent.Amount,
            Currency = allocation.Currency,
            Status = "ISSUED",
            IssuedAt = now,
            CapturedDate = now
        };
        foreach (var line in lines)
            line.InvoiceMasterID = invoice.InvoiceMasterID;

        db.InvoiceMasters.Add(invoice);
        db.InvoiceDetails.AddRange(lines);
        intent.InvoiceMasterID = invoice.InvoiceMasterID;
    }

    public async Task ApplyPaymentToInvoiceAsync(Payment payment, CancellationToken ct)
    {
        if (await db.PaymentAllocations.AnyAsync(x => x.PaymentID == payment.PaymentID, ct))
            return;

        var intent = await db.PaymentIntents.AsNoTracking()
            .SingleAsync(x => x.PaymentIntentID == payment.PaymentIntentID, ct);
        if (!intent.InvoiceMasterID.HasValue)
            return;

        var invoice = await db.InvoiceMasters.SingleOrDefaultAsync(
            x => x.InvoiceMasterID == intent.InvoiceMasterID.Value, ct)
            ?? throw new InvalidOperationException("The payment references an invoice that does not exist.");
        var balance = invoice.TotalAmount - invoice.AmountPaid;
        if (payment.GrossAmount <= 0m || payment.GrossAmount > balance)
            throw new InvalidOperationException("The confirmed payment exceeds the invoice balance.");

        var details = await db.InvoiceDetails.Where(x => x.InvoiceMasterID == invoice.InvoiceMasterID)
            .OrderBy(x => x.InvoiceDetailID).ToListAsync(ct);
        var existingAllocations = await db.PaymentAllocations.AsNoTracking()
            .Where(x => x.InvoiceMasterID == invoice.InvoiceMasterID && x.InvoiceDetailID != null)
            .Select(x => new { x.InvoiceDetailID, x.AllocatedAmount })
            .ToListAsync(ct);
        var existingByLine = existingAllocations.GroupBy(x => x.InvoiceDetailID!.Value)
            .ToDictionary(group => group.Key, group => group.Sum(x => x.AllocatedAmount));

        var remaining = payment.GrossAmount;
        var allocatedAt = payment.PaidAt ?? DateTime.UtcNow;
        var allocations = new List<PaymentAllocation>();
        foreach (var detail in details)
        {
            if (remaining == 0m)
                break;
            var lineBalance = detail.TotalLineAmount - existingByLine.GetValueOrDefault(detail.InvoiceDetailID);
            if (lineBalance < 0m)
                throw new InvalidOperationException("Existing allocations exceed an invoice line total.");
            var amount = Math.Min(remaining, lineBalance);
            if (amount <= 0m)
                continue;
            allocations.Add(new PaymentAllocation
            {
                PaymentID = payment.PaymentID,
                InvoiceMasterID = invoice.InvoiceMasterID,
                InvoiceDetailID = detail.InvoiceDetailID,
                AllocatedAmount = amount,
                AllocationType = detail.LineType ?? "LINE_ITEM",
                AllocatedAt = allocatedAt,
                CapturedDate = allocatedAt,
                CapturedBy = "SYSTEM"
            });
            remaining -= amount;
        }
        if (remaining > 0m)
        {
            allocations.Add(new PaymentAllocation
            {
                PaymentID = payment.PaymentID,
                InvoiceMasterID = invoice.InvoiceMasterID,
                AllocatedAmount = remaining,
                AllocationType = "INVOICE_BALANCE",
                AllocatedAt = allocatedAt,
                CapturedDate = allocatedAt,
                CapturedBy = "SYSTEM"
            });
        }

        db.PaymentAllocations.AddRange(allocations);
        invoice.AmountPaid += payment.GrossAmount;
        invoice.BalanceDue = invoice.TotalAmount - invoice.AmountPaid;
        invoice.Status = invoice.BalanceDue == 0m ? "PAID" : "PARTIALLY_PAID";
        invoice.PaidAt = invoice.BalanceDue == 0m ? allocatedAt : null;
        await db.SaveChangesAsync(ct);
    }

    private static InvoiceDetail CreateInvoiceLine(
        string lineType, string description, decimal amount, Guid? feeId = null,
        DateTime? servicePeriodStart = null, DateTime? servicePeriodEnd = null) =>
        new()
        {
            InvoiceDetailID = Guid.NewGuid(),
            InvoiceMasterID = Guid.Empty,
            FeeID = feeId,
            LineType = lineType,
            Description = description,
            ServicePeriodStart = servicePeriodStart,
            ServicePeriodEnd = servicePeriodEnd,
            Quantity = 1m,
            UnitAmount = amount,
            LineAmount = amount,
            TaxAmount = 0m,
            TotalLineAmount = amount,
            CapturedDate = DateTime.UtcNow
        };

    private static decimal HostFee(List<Fee> fees, string code, string name, Lease lease)
    {
        var matching = fees.Where(x => string.Equals(x.Code, code, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.FeeType?.Name, name, StringComparison.OrdinalIgnoreCase)).ToList();
        var scoped = matching.Where(x => x.OrganizationID == lease.OrganizationID).ToList();
        var applicable = scoped.Count > 0 ? scoped : matching.Where(x => x.OrganizationID == null).ToList();
        if (applicable.Count != 1)
            throw new InvalidOperationException($"Exactly one effective {name} must be configured.");
        var fee = applicable[0];
        if (!string.Equals(fee.Currency, lease.Currency, StringComparison.OrdinalIgnoreCase) ||
            fee.CalculationType?.ToUpperInvariant() is not ("FIXED" or "PERCENTAGE") ||
            (fee.CalculationType.ToUpperInvariant() == "FIXED" && !fee.FixedAmount.HasValue) ||
            (fee.CalculationType.ToUpperInvariant() == "PERCENTAGE" && !fee.PercentageRate.HasValue) ||
            fee.FixedAmount < 0 || fee.PercentageRate < 0)
            throw new InvalidOperationException($"The configured {name} is invalid for this lease.");
        return TenantBookingFees.CalculateAmount(fee, lease.BaseRentAmount);
    }

    // Called inside the successful-payment transaction, never while a debit is pending.
    public async Task SettleAsync(Payment payment, CancellationToken ct)
    {
        if (db.Database.CurrentTransaction == null)
            throw new InvalidOperationException("Rent accounting must be settled inside a payment transaction.");
        var allocation = await db.RentPaymentAccountings
            .SingleOrDefaultAsync(x => x.PaymentIntentID == payment.PaymentIntentID, ct);
        if (allocation == null)
            return; // Legacy payments require an explicit, audited reconciliation.
        if (allocation.SettledAt.HasValue)
            return;
        if (payment.Status != "PAID" || payment.RefundedAmount != 0m ||
            payment.GrossAmount != allocation.RentAmount + allocation.DepositAmount + allocation.TenantFeeAmount)
            throw new InvalidOperationException("Only a successful, matching, unrefunded payment can be allocated.");
        if (db.Database.IsSqlServer())
        {
            await db.RentalUnits.FromSqlInterpolated(
                $"SELECT * FROM [RentalUnits] WITH (UPDLOCK, HOLDLOCK) WHERE [RentalUnitID] = {allocation.RentalUnitID}")
                .AsNoTracking().ToListAsync(ct);
            await db.Leases.FromSqlInterpolated(
                $"SELECT * FROM [Leases] WITH (UPDLOCK, HOLDLOCK) WHERE [LeaseID] = {allocation.LeaseID}")
                .AsNoTracking().ToListAsync(ct);
        }

        var settled = db.RentPaymentAccountings.AsNoTracking()
            .Where(x => x.LeaseID == allocation.LeaseID && x.SettledAt != null);
        allocation.HostPlacementFee = await settled.AnyAsync(ct)
            ? 0m : allocation.PlacementFeeRate;
        allocation.HostMonthlyFee = await db.RentPaymentAccountings.AsNoTracking().AnyAsync(
            x => x.RentalUnitID == allocation.RentalUnitID && x.SettledAt != null &&
                x.RentPeriod == allocation.RentPeriod, ct)
            ? 0m : allocation.MonthlyFeeRate;
        allocation.HostNetAmount = allocation.RentAmount - allocation.HostPlacementFee - allocation.HostMonthlyFee;
        allocation.PaymentID = payment.PaymentID;
        allocation.SettledAt = payment.PaidAt ?? DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }
}
