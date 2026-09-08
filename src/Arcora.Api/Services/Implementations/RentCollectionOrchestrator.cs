using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Payments.Abstractions;
using Microsoft.Extensions.Logging;

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

        public RentCollectionOrchestrator(
            IMapper mapper,
            ILogger<RentCollectionOrchestrator> logger,
            IPaymentProvider paymentProvider,
            IPaymentIntentRepository paymentIntentRepository,
            IPaymentAttemptRepository paymentAttemptRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IPaymentRepository paymentRepository,
            IAutopayMandateRepository autopayMandateRepository)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.paymentProvider = paymentProvider;
            this.paymentIntentRepository = paymentIntentRepository;
            this.paymentAttemptRepository = paymentAttemptRepository;
            this.paymentMethodRepository = paymentMethodRepository;
            this.paymentRepository = paymentRepository;
            this.autopayMandateRepository = autopayMandateRepository;
        }

        /// <inheritdoc/>
        public async Task<PaymentIntentDto> InitiateCollectionAsync(Guid? leaseId, Guid tenantId, Guid invoiceMasterId, decimal amount, string? idempotencyKey = null, CancellationToken cancellationToken = default)
        {
            var padMethod = await GetTenantMethodByRoleAsync(tenantId, "PRIMARY")
                ?? throw new InvalidOperationException($"Tenant {tenantId} has no verified PAD (PRIMARY) payment method.");

            var mandates = leaseId.HasValue
                ? await autopayMandateRepository.Find(x => x.LeaseID == leaseId && x.TenantID == tenantId && x.Status == "ACTIVE")
                : null;
            var mandate = mandates?.FirstOrDefault();

            // Fall back to the tenant-level PAD mandate established during onboarding (not yet lease-bound).
            if (mandate == null)
            {
                var tenantMandates = await autopayMandateRepository.Find(x => x.TenantID == tenantId && x.PaymentRail == "PAD" && x.Status == "ACTIVE");
                mandate = tenantMandates?.FirstOrDefault();
            }

            var key = string.IsNullOrWhiteSpace(idempotencyKey)
                ? $"pi_{(leaseId.HasValue ? leaseId.Value.ToString("N") : tenantId.ToString("N"))}_{invoiceMasterId:N}"
                : idempotencyKey;

            var intent = new PaymentIntent
            {
                PaymentIntentID = Guid.NewGuid(),
                InvoiceMasterID = invoiceMasterId == Guid.Empty ? null : invoiceMasterId,
                LeaseID = leaseId,
                TenantID = tenantId,
                PaymentMethodID = padMethod.PaymentMethodID,
                AutopayMandateID = mandate?.AutopayMandateID,
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

            await paymentIntentRepository.Create(intent);
            await paymentIntentRepository.Save();

            await AttemptChargeAsync(intent, padMethod, PaymentMethodKind.Pad, mandate?.ProviderMandateID, cancellationToken);

            return mapper.Map<PaymentIntentDto>(intent);
        }

        /// <inheritdoc/>
        public async Task HandlePadResultAsync(Guid paymentIntentId, string providerStatus, string? failureCode, string? failureMessage, CancellationToken cancellationToken = default)
        {
            var intent = await paymentIntentRepository.GetByID(paymentIntentId);
            if (intent == null)
            {
                logger.LogWarning("PAD result received for unknown PaymentIntent {Id}", paymentIntentId);
                return;
            }

            var category = Arcora.Payments.Orchestration.FailureClassifier.Classify(providerStatus, failureCode);

            switch (category)
            {
                case FailureCategory.None:
                    await MarkPaidAsync(intent, providerStatus, cancellationToken);
                    break;

                case FailureCategory.Pending:
                    // Not final yet - do NOT fall back to the card. Wait for the next webhook.
                    intent.Status = "PROCESSING";
                    intent.FailureCategory = category.ToString().ToUpperInvariant();
                    await UpdateIntentAsync(intent);
                    break;

                case FailureCategory.Temporary:
                case FailureCategory.Permanent:
                    intent.FailureCode = failureCode;
                    intent.FailureReason = failureMessage;
                    intent.FailureCategory = category.ToString().ToUpperInvariant();
                    intent.Status = "PAD_FAILED";
                    await UpdateIntentAsync(intent);

                    // Definitive PAD failure -> charge the backup card.
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
                attempt.Status = "FAILED";
                attempt.FailureMessage = ex.Message;
                attempt.FailureCategory = FailureCategory.Permanent.ToString().ToUpperInvariant();
                attempt.CompletedAt = DateTime.UtcNow;
                await paymentAttemptRepository.Create(attempt);
                await paymentAttemptRepository.Save();
                return FailureCategory.Permanent;
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
                await MarkPaidAsync(intent, charge.Status, cancellationToken);
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

        private async Task MarkPaidAsync(PaymentIntent intent, string? providerStatus, CancellationToken cancellationToken)
        {
            intent.Status = "PAID";
            intent.FailureCategory = FailureCategory.None.ToString().ToUpperInvariant();
            intent.CompletedAt = DateTime.UtcNow;
            await UpdateIntentAsync(intent);

            var existing = await paymentRepository.Find(x => x.PaymentIntentID == intent.PaymentIntentID);
            if (existing != null && existing.Any())
                return;

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
