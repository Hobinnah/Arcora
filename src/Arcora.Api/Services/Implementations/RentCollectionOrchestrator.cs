using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Payments.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Arcora.Api.Enums;

namespace Arcora.Api.Services.Implementations
{
    /// <summary>
    /// Implements the PAD-primary / credit-card-backup rent collection state machine.
    /// See <see cref="IRentCollectionOrchestrator"/> for the double-charge safety rationale.
    /// </summary>
    public class RentCollectionOrchestrator : IRentCollectionOrchestrator
    {
        private const string ProviderName = "STRIPE";

        private readonly IMapper mapper;
        private readonly ILogger<RentCollectionOrchestrator> logger;
        private readonly IPaymentProvider paymentProvider;
        private readonly IPaymentIntentRepository paymentIntentRepository;
        private readonly IPaymentAttemptRepository paymentAttemptRepository;
        private readonly IPaymentMethodRepository paymentMethodRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IAutopayMandateRepository autopayMandateRepository;
        private readonly ArcoraDbContext dbContext;
        private readonly HybridCache cache;

        public RentCollectionOrchestrator(
            IMapper mapper,
            ILogger<RentCollectionOrchestrator> logger,
            IPaymentProvider paymentProvider,
            IPaymentIntentRepository paymentIntentRepository,
            IPaymentAttemptRepository paymentAttemptRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IPaymentRepository paymentRepository,
            IAutopayMandateRepository autopayMandateRepository,
            ArcoraDbContext dbContext,
            HybridCache cache)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.paymentProvider = paymentProvider;
            this.paymentIntentRepository = paymentIntentRepository;
            this.paymentAttemptRepository = paymentAttemptRepository;
            this.paymentMethodRepository = paymentMethodRepository;
            this.paymentRepository = paymentRepository;
            this.autopayMandateRepository = autopayMandateRepository;
            this.dbContext = dbContext;
            this.cache = cache;
        }

        /// <inheritdoc/>
        public async Task<PaymentIntentDto> InitiateCollectionAsync(Guid? leaseId, Guid tenantId, Guid invoiceMasterId, decimal amount, string? idempotencyKey = null, CancellationToken cancellationToken = default)
        {
            if (amount <= 0m)
                throw new InvalidOperationException("Collection amount must be positive.");
            var key = string.IsNullOrWhiteSpace(idempotencyKey)
                ? $"pi_{(leaseId.HasValue ? leaseId.Value.ToString("N") : tenantId.ToString("N"))}_{invoiceMasterId:N}"
                : idempotencyKey;
            var existing = await dbContext.PaymentIntents.AsNoTracking()
                .SingleOrDefaultAsync(item => item.IdempotencyKey == key, cancellationToken);
            if (existing != null)
                return ExistingIntent(existing, leaseId, tenantId, invoiceMasterId, amount);

            var padMethod = await GetTenantMethodByRoleAsync(tenantId, "PRIMARY")
                ?? throw new InvalidOperationException($"Tenant {tenantId} has no verified PAD (PRIMARY) payment method.");

            var mandates = leaseId.HasValue
                ? await autopayMandateRepository.Find(x => x.LeaseID == leaseId && x.TenantID == tenantId &&
                    x.PaymentMethodID == padMethod.PaymentMethodID && x.PaymentRail == "PAD" && x.Status == "ACTIVE")
                : null;
            var mandate = mandates?.FirstOrDefault();

            // Fall back to the tenant-level PAD mandate established during onboarding (not yet lease-bound).
            if (mandate == null)
            {
                var tenantMandates = await autopayMandateRepository.Find(x => x.TenantID == tenantId &&
                    x.LeaseID == null && x.PaymentMethodID == padMethod.PaymentMethodID && x.PaymentRail == "PAD" && x.Status == "ACTIVE");
                mandate = tenantMandates?.FirstOrDefault();
            }

            if (mandate == null)
                throw new InvalidOperationException("An active PAD mandate is required for collection.");

            var intent = new PaymentIntent
            {
                PaymentIntentID = Guid.NewGuid(),
                InvoiceMasterID = invoiceMasterId == Guid.Empty ? null : invoiceMasterId,
                LeaseID = leaseId,
                TenantID = tenantId,
                PaymentMethodID = padMethod.PaymentMethodID,
                AutopayMandateID = mandate.AutopayMandateID,
                Amount = amount,
                Currency = "CAD",
                Status = "PROCESSING",
                CollectionMethod = "AUTOPAY",
                ProviderName = ProviderName,
                IdempotencyKey = key,
                ScheduledChargeAt = DateTime.UtcNow,
                StartedAt = DateTime.UtcNow,
                CapturedDate = DateTime.UtcNow
            };

            var accounting = new RentPaymentAccountingService(dbContext);
            var allocation = await accounting.SnapshotAsync(intent, cancellationToken);
            var generatedInvoice = allocation != null && !intent.InvoiceMasterID.HasValue;
            if (allocation != null)
                await accounting.CreateInvoiceAsync(intent, allocation, cancellationToken);
            var generatedInvoiceId = generatedInvoice ? intent.InvoiceMasterID : null;
            await paymentIntentRepository.Create(intent);
            if (allocation != null)
                dbContext.RentPaymentAccountings.Add(allocation);
            try
            {
                await paymentIntentRepository.Save();
            }
            catch (DbUpdateException)
            {
                dbContext.Entry(intent).State = EntityState.Detached;
                if (allocation != null)
                    dbContext.Entry(allocation).State = EntityState.Detached;
                if (generatedInvoiceId.HasValue)
                {
                    foreach (var detailEntry in dbContext.ChangeTracker.Entries<InvoiceDetail>()
                        .Where(entry => entry.State == EntityState.Added &&
                            entry.Entity.InvoiceMasterID == generatedInvoiceId.Value)
                        .ToList())
                        detailEntry.State = EntityState.Detached;
                    var invoiceEntry = dbContext.ChangeTracker.Entries<InvoiceMaster>()
                        .FirstOrDefault(entry => entry.State == EntityState.Added &&
                            entry.Entity.InvoiceMasterID == generatedInvoiceId.Value);
                    if (invoiceEntry != null)
                        invoiceEntry.State = EntityState.Detached;
                }
                var concurrent = await dbContext.PaymentIntents.AsNoTracking()
                    .SingleOrDefaultAsync(item => item.IdempotencyKey == key, cancellationToken);
                if (concurrent == null)
                    throw;
                logger.LogInformation("Reusing concurrent collection intent {IntentId}", concurrent.PaymentIntentID);
                return ExistingIntent(concurrent, leaseId, tenantId, invoiceMasterId, amount);
            }
            InvalidatePaymentCaches();

            var result = await AttemptChargeAsync(intent, padMethod, PaymentMethodKind.Pad, mandate.ProviderMandateID, cancellationToken);
            if (result is FailureCategory.Temporary or FailureCategory.Permanent)
                await ChargeBackupCardAsync(intent.PaymentIntentID, cancellationToken);

            return mapper.Map<PaymentIntentDto>(intent);
        }

        private PaymentIntentDto ExistingIntent(PaymentIntent intent, Guid? leaseId, Guid tenantId, Guid invoiceId, decimal amount)
        {
            if (intent.LeaseID != leaseId || intent.TenantID != tenantId || intent.Amount != amount ||
                (invoiceId != Guid.Empty && intent.InvoiceMasterID != invoiceId))
                throw new InvalidOperationException("The collection idempotency key belongs to a different payment.");
            return mapper.Map<PaymentIntentDto>(intent);
        }

        /// <inheritdoc/>
        public async Task HandlePadResultAsync(
            Guid paymentIntentId, string providerStatus, string? failureCode, string? failureMessage,
            CancellationToken cancellationToken = default, string? providerPaymentIntentId = null)
        {
            var intent = await paymentIntentRepository.GetByID(paymentIntentId);
            if (intent == null)
            {
                logger.LogWarning("PAD result received for unknown PaymentIntent {Id}", paymentIntentId);
                return;
            }

            var category = Arcora.Payments.Orchestration.FailureClassifier.Classify(providerStatus, failureCode);
            var providerIntentId = string.IsNullOrWhiteSpace(providerPaymentIntentId)
                ? intent.ProviderPaymentIntentID
                : providerPaymentIntentId;
            if (category != FailureCategory.None &&
                !string.IsNullOrWhiteSpace(providerIntentId) &&
                !string.Equals(providerIntentId, intent.ProviderPaymentIntentID, StringComparison.Ordinal))
            {
                await UpdateAttemptResultAsync(intent.PaymentIntentID, providerIntentId, category,
                    failureCode, failureMessage, cancellationToken);
                return;
            }
            if (intent.Status == "PAID")
            {
                if (category == FailureCategory.None)
                    await MarkPaidAsync(intent, providerStatus, providerIntentId, cancellationToken);
                return;
            }
            if (intent.Status is "OVERDUE" or "CARD_FAILED")
                return;

            switch (category)
            {
                case FailureCategory.None:
                    await MarkPaidAsync(intent, providerStatus, providerIntentId, cancellationToken);
                    break;

                case FailureCategory.Pending:
                    // Not final yet - do NOT fall back to the card. Wait for the next webhook.
                    await UpdateAttemptResultAsync(intent.PaymentIntentID, providerIntentId, category,
                        failureCode, failureMessage, cancellationToken);
                    intent.Status = "PROCESSING";
                    intent.FailureCategory = category.ToString().ToUpperInvariant();
                    await UpdateIntentAsync(intent);
                    break;

                case FailureCategory.Temporary:
                case FailureCategory.Permanent:
                    var attemptMethodKind = string.IsNullOrWhiteSpace(providerIntentId)
                        ? await dbContext.PaymentAttempts.AsNoTracking()
                            .Where(attempt => attempt.PaymentIntentID == intent.PaymentIntentID)
                            .OrderByDescending(attempt => attempt.AttemptNumber)
                            .Select(attempt => attempt.MethodKind).FirstOrDefaultAsync(cancellationToken)
                        : await dbContext.PaymentAttempts.AsNoTracking()
                            .Where(attempt => attempt.PaymentIntentID == intent.PaymentIntentID &&
                                attempt.ProviderAttemptID == providerIntentId)
                            .Select(attempt => attempt.MethodKind).FirstOrDefaultAsync(cancellationToken);
                    await UpdateAttemptResultAsync(intent.PaymentIntentID, providerIntentId, category,
                        failureCode, failureMessage, cancellationToken);
                    intent.FailureCode = failureCode;
                    intent.FailureReason = failureMessage;
                    intent.FailureCategory = category.ToString().ToUpperInvariant();
                    intent.Status = attemptMethodKind == "CARD" ? "OVERDUE" : "PAD_FAILED";
                    await UpdateIntentAsync(intent);

                    // Definitive PAD failure -> charge the backup card.
                    if (intent.Status == "PAD_FAILED")
                        await ChargeBackupCardAsync(intent.PaymentIntentID, cancellationToken);
                    break;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ChargeBackupCardAsync(Guid paymentIntentId, CancellationToken cancellationToken = default)
        {
            var intent = await paymentIntentRepository.GetByID(paymentIntentId);
            if (intent == null)
                return false;

            var claimed = await dbContext.PaymentIntents
                .Where(item => item.PaymentIntentID == paymentIntentId && item.Status == "PAD_FAILED")
                .ExecuteUpdateAsync(update => update.SetProperty(item => item.Status, "BACKUP_PROCESSING"), cancellationToken);
            if (claimed == 0)
                return false;
            intent.Status = "BACKUP_PROCESSING";

            var cardMethod = await GetTenantMethodByRoleAsync(intent.TenantID, "BACKUP");
            if (cardMethod == null)
            {
                logger.LogWarning("No backup card for tenant {TenantId}; marking intent {IntentId} OVERDUE", intent.TenantID, intent.PaymentIntentID);
                intent.Status = "OVERDUE";
                await UpdateIntentAsync(intent);
                return false;
            }

            var result = await AttemptChargeAsync(intent, cardMethod, PaymentMethodKind.Card, null, cancellationToken);
            if (result == FailureCategory.None)
                return true;
            if (result == FailureCategory.Pending)
                return false;

            intent.Status = "OVERDUE";
            await UpdateIntentAsync(intent);
            return false;
        }

        private async Task<FailureCategory> AttemptChargeAsync(PaymentIntent intent, PaymentMethod method, PaymentMethodKind kind, string? mandateId, CancellationToken cancellationToken)
        {
            var attemptNumber = await NextAttemptNumberAsync(intent.PaymentIntentID);
            var attempt = new PaymentAttempt
            {
                PaymentAttemptID = Guid.NewGuid(),
                PaymentIntentID = intent.PaymentIntentID,
                AttemptNumber = attemptNumber,
                PaymentMethodID = method.PaymentMethodID,
                MethodKind = kind == PaymentMethodKind.Pad ? "PAD" : "CARD",
                Amount = intent.Amount,
                Status = "PENDING",
                AttemptedAt = DateTime.UtcNow,
                CapturedDate = DateTime.UtcNow
            };

            var request = new CreateChargeRequest(
                CustomerId: method.ProviderCustomerID!,
                PaymentMethodId: method.ProviderPaymentMethodID!,
                Kind: kind,
                AmountInMinorUnits: (long)Math.Round(intent.Amount * 100m, MidpointRounding.AwayFromZero),
                Currency: (intent.Currency ?? "CAD").ToLowerInvariant(),
                IdempotencyKey: $"{intent.IdempotencyKey}_a{attemptNumber}",
                MandateId: mandateId,
                Description: $"Rent collection for lease {intent.LeaseID}");

            ProviderChargeResult charge;
            try
            {
                charge = await paymentProvider.CreateChargeAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Provider charge threw for intent {IntentId}", intent.PaymentIntentID);
                attempt.Status = "UNKNOWN";
                attempt.FailureMessage = ex.Message;
                attempt.FailureCategory = FailureCategory.Pending.ToString().ToUpperInvariant();
                attempt.CompletedAt = DateTime.UtcNow;
                await paymentAttemptRepository.Create(attempt);
                await paymentAttemptRepository.Save();
                intent.FailureCategory = "PENDING";
                intent.FailureReason = "The provider outcome is uncertain. Do not retry the debit; refresh payment status or contact support.";
                await UpdateIntentAsync(intent);
                return FailureCategory.Pending;
            }

            attempt.ProviderAttemptID = charge.PaymentIntentId;
            attempt.FailureCode = charge.FailureCode;
            attempt.FailureMessage = charge.FailureMessage;
            attempt.FailureCategory = charge.FailureCategory.ToString().ToUpperInvariant();
            attempt.CompletedAt = DateTime.UtcNow;

            intent.ProviderPaymentIntentID = charge.PaymentIntentId;

            if (charge.FailureCategory == FailureCategory.None)
            {
                attempt.Status = "SUCCEEDED";
                await paymentAttemptRepository.Create(attempt);
                await paymentAttemptRepository.Save();
                await MarkPaidAsync(intent, charge.Status, charge.PaymentIntentId, cancellationToken);
            }
            else if (charge.FailureCategory == FailureCategory.Pending)
            {
                // Awaiting definitive result (typical for ACSS PAD). Persist and wait for webhook.
                attempt.Status = "PROCESSING";
                await paymentAttemptRepository.Create(attempt);
                await paymentAttemptRepository.Save();
                intent.Status = "PROCESSING";
                intent.FailureCategory = FailureCategory.Pending.ToString().ToUpperInvariant();
                await UpdateIntentAsync(intent);
            }
            else
            {
                attempt.Status = "FAILED";
                await paymentAttemptRepository.Create(attempt);
                await paymentAttemptRepository.Save();
                intent.FailureCode = charge.FailureCode;
                intent.FailureReason = charge.FailureMessage;
                intent.FailureCategory = charge.FailureCategory.ToString().ToUpperInvariant();
                intent.Status = kind == PaymentMethodKind.Pad ? "PAD_FAILED" : "CARD_FAILED";
                await UpdateIntentAsync(intent);
            }

            return charge.FailureCategory;
        }

        private async Task MarkPaidAsync(
            PaymentIntent intent, string? providerStatus, string? providerPaymentIntentId,
            CancellationToken cancellationToken)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable, cancellationToken);
            if (dbContext.Database.IsSqlServer())
                await dbContext.PaymentIntents.FromSqlInterpolated(
                    $"SELECT * FROM [PaymentIntents] WITH (UPDLOCK, HOLDLOCK) WHERE [PaymentIntentID] = {intent.PaymentIntentID}")
                    .AsNoTracking().ToListAsync(cancellationToken);
            await UpdateAttemptResultAsync(intent.PaymentIntentID, providerPaymentIntentId,
                FailureCategory.None, null, null, cancellationToken);
            intent.Status = "PAID";
            intent.FailureCategory = FailureCategory.None.ToString().ToUpperInvariant();
            intent.CompletedAt ??= DateTime.UtcNow;
            await UpdateIntentAsync(intent);

            var existing = await paymentRepository.Find(x => x.PaymentIntentID == intent.PaymentIntentID);
            var existingPayment = existing?.FirstOrDefault(x => x != null);
            if (existingPayment != null)
            {
                await new RentPaymentAccountingService(dbContext)
                    .ApplyPaymentToInvoiceAsync(existingPayment, cancellationToken);
                await new RentPaymentAccountingService(dbContext).SettleAsync(existingPayment, cancellationToken);
                await RecordFirstChargeDepositFundingAsync(intent, existingPayment.PaymentID, cancellationToken);
                await ActivateLeaseIfEligibleAsync(intent.LeaseID, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                InvalidatePaymentCaches();
                return;
            }

            var payment = new Payment
            {
                PaymentID = Guid.NewGuid(),
                PaymentIntentID = intent.PaymentIntentID,
                TenantID = intent.TenantID,
                ProviderChargeID = intent.ProviderPaymentIntentID,
                Status = "PAID",
                GrossAmount = intent.Amount,
                NetAmount = intent.Amount,
                Currency = intent.Currency,
                PaidAt = DateTime.UtcNow,
                CapturedDate = DateTime.UtcNow
            };
            await paymentRepository.Create(payment);
            await paymentRepository.Save();

            var paymentAccounting = new RentPaymentAccountingService(dbContext);
            await paymentAccounting.ApplyPaymentToInvoiceAsync(payment, cancellationToken);
            await paymentAccounting.SettleAsync(payment, cancellationToken);
            await RecordFirstChargeDepositFundingAsync(intent, payment.PaymentID, cancellationToken);
            await ActivateLeaseIfEligibleAsync(intent.LeaseID, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            InvalidatePaymentCaches();
        }

        private void InvalidatePaymentCaches()
        {
            cache.Remove(Cache.INVOICEMASTERS.ToString());
            cache.Remove(Cache.INVOICEDETAILS.ToString());
            cache.Remove(Cache.PAYMENTALLOCATIONS.ToString());
            cache.Remove(Cache.PAYMENTS.ToString());
        }

        private Task<int> UpdateAttemptResultAsync(
            Guid paymentIntentId, string? providerPaymentIntentId, FailureCategory category,
            string? failureCode, string? failureMessage, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(providerPaymentIntentId))
                return Task.FromResult(0);

            var query = dbContext.PaymentAttempts.Where(attempt =>
                attempt.PaymentIntentID == paymentIntentId &&
                attempt.ProviderAttemptID == providerPaymentIntentId);
            var completedAt = DateTime.UtcNow;
            return category switch
            {
                FailureCategory.None => query.Where(attempt => attempt.Status != "SUCCEEDED")
                    .ExecuteUpdateAsync(update => update
                        .SetProperty(attempt => attempt.Status, "SUCCEEDED")
                        .SetProperty(attempt => attempt.FailureCode, (string?)null)
                        .SetProperty(attempt => attempt.FailureMessage, (string?)null)
                        .SetProperty(attempt => attempt.FailureCategory, "NONE")
                        .SetProperty(attempt => attempt.CompletedAt, attempt => attempt.CompletedAt ?? completedAt),
                        cancellationToken),
                FailureCategory.Pending => query.Where(attempt =>
                        attempt.Status == "PENDING" || attempt.Status == "PROCESSING" || attempt.Status == "UNKNOWN")
                    .ExecuteUpdateAsync(update => update
                        .SetProperty(attempt => attempt.Status, "PROCESSING")
                        .SetProperty(attempt => attempt.FailureCode, failureCode)
                        .SetProperty(attempt => attempt.FailureMessage, failureMessage)
                        .SetProperty(attempt => attempt.FailureCategory, "PENDING"),
                        cancellationToken),
                FailureCategory.Temporary or FailureCategory.Permanent => query.Where(attempt =>
                        attempt.Status != "SUCCEEDED" && attempt.Status != "FAILED")
                    .ExecuteUpdateAsync(update => update
                        .SetProperty(attempt => attempt.Status, "FAILED")
                        .SetProperty(attempt => attempt.FailureCode, failureCode)
                        .SetProperty(attempt => attempt.FailureMessage, failureMessage)
                        .SetProperty(attempt => attempt.FailureCategory, category.ToString().ToUpperInvariant())
                        .SetProperty(attempt => attempt.CompletedAt, attempt => attempt.CompletedAt ?? completedAt),
                        cancellationToken),
                _ => Task.FromResult(0)
            };
        }

        private async Task RecordFirstChargeDepositFundingAsync(PaymentIntent intent, Guid paymentID, CancellationToken cancellationToken)
        {
            var idempotencyKey = intent.IdempotencyKey;
            if (!intent.LeaseID.HasValue || string.IsNullOrWhiteSpace(idempotencyKey)
                || !idempotencyKey.StartsWith("pi_firstcharge_", StringComparison.Ordinal))
                return;

            var deposit = await dbContext.SecurityDeposits
                .FirstOrDefaultAsync(x => x.LeaseID == intent.LeaseID && x.RequiredAmount > x.ReceivedAmount, cancellationToken);
            if (deposit == null)
                return;

            var alreadyLinked = await dbContext.SecurityDepositTransactions.AnyAsync(
                x => x.SecurityDepositID == deposit.SecurityDepositID && x.PaymentID == paymentID
                    && x.TransactionType == "FUNDING",
                cancellationToken);
            if (alreadyLinked)
                return;

            var amount = Math.Min(deposit.RequiredAmount - deposit.ReceivedAmount, intent.Amount);
            if (amount <= 0m)
                return;

            var now = DateTime.UtcNow;
            deposit.ReceivedAmount += amount;
            deposit.Status = deposit.ReceivedAmount >= deposit.RequiredAmount ? "HELD" : "PARTIALLY_RECEIVED";
            deposit.HeldAt ??= now;
            if (deposit.ReceivedAmount >= deposit.RequiredAmount)
                deposit.FullyFundedAt ??= now;
            deposit.UpdatedDate = now;
            deposit.UpdatedBy = "PAYMENT_ORCHESTRATOR";

            dbContext.SecurityDepositTransactions.Add(new SecurityDepositTransaction
            {
                SecurityDepositTransactionID = Guid.NewGuid(),
                SecurityDepositID = deposit.SecurityDepositID,
                PaymentID = paymentID,
                TransactionType = "FUNDING",
                Amount = amount,
                Currency = deposit.Currency,
                Description = "Security deposit funding",
                OccurredAt = now,
                CapturedDate = now,
                CapturedBy = "PAYMENT_ORCHESTRATOR"
            });
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task ActivateLeaseIfEligibleAsync(Guid? leaseID, CancellationToken cancellationToken)
        {
            if (!leaseID.HasValue || leaseID.Value == Guid.Empty)
                return;

            var lease = await dbContext.Leases.FirstOrDefaultAsync(x => x.LeaseID == leaseID.Value, cancellationToken);
            if (lease == null)
                return;

            if (string.Equals(lease.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                return;

            if (!string.Equals(lease.Status, "PENDING_FIRST_PAYMENT", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(lease.Status, "PENDING_TENANT_SIGNATURE", StringComparison.OrdinalIgnoreCase))
                return;

            lease.Status = "ACTIVE";
            lease.ActivatedAt ??= DateTime.UtcNow;
            lease.UpdatedDate = DateTime.UtcNow;
            lease.UpdatedBy = "PAYMENT_ORCHESTRATOR";

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task UpdateIntentAsync(PaymentIntent intent)
        {
            intent.UpdatedDate = DateTime.UtcNow;
            await paymentIntentRepository.Update(intent);
            await paymentIntentRepository.Save();
        }

        private async Task<int> NextAttemptNumberAsync(Guid paymentIntentId)
        {
            var attempts = await paymentAttemptRepository.Find(x => x.PaymentIntentID == paymentIntentId);
            return (attempts?.Count() ?? 0) + 1;
        }

        private async Task<PaymentMethod?> GetTenantMethodByRoleAsync(Guid tenantId, string role)
        {
            var methods = await paymentMethodRepository.Find(x =>
                x.TenantID == tenantId &&
                x.IsActive &&
                x.MethodRole == role &&
                x.VerificationStatus == "VERIFIED");
            return methods?.FirstOrDefault();
        }
    }
}
