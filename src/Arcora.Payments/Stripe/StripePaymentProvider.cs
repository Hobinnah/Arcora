using Arcora.Payments.Abstractions;
using Arcora.Payments.Configuration;
using Arcora.Payments.Orchestration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace Arcora.Payments.Stripe
{
    /// <summary>
    /// Stripe implementation of <see cref="IPaymentProvider"/>. All raw instrument data (card numbers,
    /// bank accounts) is tokenized client-side by Stripe.js/SDK; this server only ever handles opaque
    /// Stripe identifiers (cus_, pm_, pi_, ma_) and mandate references.
    /// </summary>
    public sealed class StripePaymentProvider : IPaymentProvider
    {
        private readonly StripeOptions _options;
        private readonly ILogger<StripePaymentProvider> _logger;
        private readonly CustomerService _customers;
        private readonly SetupIntentService _setupIntents;
        private readonly PaymentMethodService _paymentMethods;
        private readonly PaymentIntentService _paymentIntents;

        public StripePaymentProvider(IOptions<StripeOptions> options, ILogger<StripePaymentProvider> logger)
        {
            _options = options.Value;
            _logger = logger;

            if (string.IsNullOrWhiteSpace(_options.SecretKey))
                throw new InvalidOperationException("Stripe SecretKey is not configured.");

            var client = new StripeClient(_options.SecretKey);
            _customers = new CustomerService(client);
            _setupIntents = new SetupIntentService(client);
            _paymentMethods = new PaymentMethodService(client);
            _paymentIntents = new PaymentIntentService(client);
        }

        /// <inheritdoc/>
        public string ProviderName => _options.ProviderName;

        /// <inheritdoc/>
        public async Task<ProviderCustomer> EnsureCustomerAsync(
            string? existingCustomerId,
            string tenantReference,
            string? email,
            string? name,
            CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(existingCustomerId))
            {
                return new ProviderCustomer(existingCustomerId);
            }

            var options = new CustomerCreateOptions
            {
                Email = email,
                Name = name,
                Metadata = new Dictionary<string, string> { ["tenant_reference"] = tenantReference }
            };

            var customer = await _customers.CreateAsync(options, cancellationToken: cancellationToken);
            _logger.LogInformation("Created Stripe customer {CustomerId} for tenant {TenantRef}", customer.Id, tenantReference);
            return new ProviderCustomer(customer.Id);
        }

        /// <inheritdoc/>
        public async Task<ProviderSetupIntent> CreateSetupIntentAsync(
            string customerId,
            PaymentMethodKind kind,
            CancellationToken cancellationToken = default)
        {
            var setupOptions = new SetupIntentCreateOptions
            {
                Customer = customerId,
                Usage = "off_session",
                PaymentMethodTypes = new List<string>
                {
                    kind == PaymentMethodKind.Pad ? "acss_debit" : "card"
                }
            };

            if (kind == PaymentMethodKind.Pad)
            {
                setupOptions.PaymentMethodOptions = new SetupIntentPaymentMethodOptionsOptions
                {
                    AcssDebit = new SetupIntentPaymentMethodOptionsAcssDebitOptions
                    {
                        Currency = "cad",
                        MandateOptions = new SetupIntentPaymentMethodOptionsAcssDebitMandateOptionsOptions
                        {
                            PaymentSchedule = "interval",
                            IntervalDescription = "Monthly rent",
                            TransactionType = "personal"
                        }
                    }
                };
            }

            var setupIntent = await _setupIntents.CreateAsync(setupOptions, cancellationToken: cancellationToken);
            _logger.LogInformation("Created Stripe SetupIntent {SetupIntentId} for customer {CustomerId}", setupIntent.Id, customerId);
            return new ProviderSetupIntent(setupIntent.Id, setupIntent.ClientSecret, setupIntent.Status);
        }

        /// <inheritdoc/>
        public async Task<ProviderPaymentMethodResult> SetupPaymentMethodAsync(
            SetupPaymentMethodRequest request,
            CancellationToken cancellationToken = default)
        {
            // ACSS debit (PAD) payment methods are attached to the customer and verified when the client
            // confirms the SetupIntent created during onboarding (mandate collection happens there). By the
            // time we reach this point the PAD method is already attached/verified, so we must NOT attach it
            // again or create a second server-side SetupIntent - doing so throws "must attach it to a
            // Customer first". We simply retrieve the already-verified payment method.
            if (request.Kind == PaymentMethodKind.Pad)
            {
                var padPm = await _paymentMethods.GetAsync(request.PaymentMethodId, cancellationToken: cancellationToken);
                var verificationStatus = padPm.Customer != null ? "succeeded" : "requires_action";
                return MapPaymentMethod(request.Kind, padPm, verificationStatus);
            }

            // Attach the (already client-tokenized) card to the customer for off-session reuse.
            await _paymentMethods.AttachAsync(
                request.PaymentMethodId,
                new PaymentMethodAttachOptions { Customer = request.CustomerId },
                cancellationToken: cancellationToken);

            var setupOptions = new SetupIntentCreateOptions
            {
                Customer = request.CustomerId,
                PaymentMethod = request.PaymentMethodId,
                Confirm = true,
                Usage = "off_session",
                PaymentMethodTypes = new List<string> { "card" }
            };

            var idempotencyKey = request.IdempotencyKey;
            var setupIntent = await _setupIntents.CreateAsync(
                setupOptions,
                new RequestOptions { IdempotencyKey = idempotencyKey },
                cancellationToken);

            var pm = await _paymentMethods.GetAsync(request.PaymentMethodId, cancellationToken: cancellationToken);
            return MapPaymentMethod(request.Kind, pm, setupIntent.Status);
        }

        /// <inheritdoc/>
        public async Task<ProviderMandateResult> CreatePadMandateAsync(
            string customerId,
            string paymentMethodId,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default)
        {
            // For ACSS/PAD the mandate is created (and the payment method attached to the customer)
            // when the CLIENT confirms the onboarding SetupIntent - not here. The payment method has
            // therefore already been consumed by that confirmation, so we must NOT create a second
            // server-side SetupIntent with Confirm = true: Stripe rejects it with "The provided
            // PaymentMethod cannot be attached. To reuse a PaymentMethod, you must attach it to a
            // Customer first." Instead we locate the already-confirmed SetupIntent for this
            // customer/payment method and surface its existing mandate reference for persistence.
            var setupIntents = await _setupIntents.ListAsync(
                new SetupIntentListOptions
                {
                    Customer = customerId,
                    PaymentMethod = paymentMethodId,
                    Limit = 1
                },
                cancellationToken: cancellationToken);

            var setupIntent = setupIntents.Data.FirstOrDefault();

            if (setupIntent == null)
            {
                throw new InvalidOperationException(
                    $"No confirmed SetupIntent was found for payment method '{paymentMethodId}'. " +
                    "The PAD mandate must be collected by confirming the onboarding SetupIntent on the client before it can be persisted.");
            }

            return new ProviderMandateResult(
                setupIntent.MandateId ?? setupIntent.Id,
                paymentMethodId,
                setupIntent.Status);
        }

        /// <inheritdoc/>
        public async Task<ProviderChargeResult> CreateChargeAsync(
            CreateChargeRequest request,
            CancellationToken cancellationToken = default)
        {
            var options = new PaymentIntentCreateOptions
            {
                Customer = request.CustomerId,
                PaymentMethod = request.PaymentMethodId,
                Amount = request.AmountInMinorUnits,
                Currency = request.Currency,
                Confirm = true,
                OffSession = true,
                Description = request.Description,
                StatementDescriptor = _options.StatementDescriptor,
                PaymentMethodTypes = new List<string>
                {
                    request.Kind == PaymentMethodKind.Pad ? "acss_debit" : "card"
                },
                Metadata = request.Metadata?.ToDictionary(k => k.Key, v => v.Value)
            };

            if (!string.IsNullOrWhiteSpace(request.MandateId))
            {
                options.Mandate = request.MandateId;
            }

            try
            {
                var intent = await _paymentIntents.CreateAsync(
                    options,
                    new RequestOptions { IdempotencyKey = request.IdempotencyKey },
                    cancellationToken);

                return new ProviderChargeResult(
                    intent.Id,
                    intent.LatestChargeId,
                    intent.Status,
                    FailureClassifier.Classify(intent.Status, null),
                    null,
                    null);
            }
            catch (StripeException ex)
            {
                var code = ex.StripeError?.Code ?? ex.StripeError?.DeclineCode;
                _logger.LogWarning(ex, "Stripe charge failed for customer {CustomerId} with code {Code}", request.CustomerId, code);
                return new ProviderChargeResult(
                    ex.StripeError?.PaymentIntent?.Id ?? string.Empty,
                    null,
                    ex.StripeError?.PaymentIntent?.Status ?? "failed",
                    FailureClassifier.Classify(ex.StripeError?.PaymentIntent?.Status, code),
                    code,
                    ex.StripeError?.Message ?? ex.Message);
            }
        }

        /// <inheritdoc/>
        public ProviderWebhookEvent ConstructWebhookEvent(string requestBody, string signatureHeader)
        {
            var stripeEvent = EventUtility.ConstructEvent(
                requestBody,
                signatureHeader,
                _options.WebhookSecret,
                throwOnApiVersionMismatch: _options.ThrowOnApiVersionMismatch);

            string? paymentIntentId = null;
            string? chargeId = null;
            string? mandateId = null;
            string? status = null;
            string? failureCode = null;
            string? failureMessage = null;

            switch (stripeEvent.Data.Object)
            {
                case PaymentIntent pi:
                    paymentIntentId = pi.Id;
                    chargeId = pi.LatestChargeId;
                    status = pi.Status;
                    failureCode = pi.LastPaymentError?.Code ?? pi.LastPaymentError?.DeclineCode;
                    failureMessage = pi.LastPaymentError?.Message;
                    break;
                case Charge charge:
                    chargeId = charge.Id;
                    paymentIntentId = charge.PaymentIntentId;
                    status = charge.Status;
                    failureCode = charge.FailureCode;
                    failureMessage = charge.FailureMessage;
                    break;
                case Mandate mandate:
                    mandateId = mandate.Id;
                    status = mandate.Status;
                    break;
            }

            return new ProviderWebhookEvent(
                stripeEvent.Id,
                stripeEvent.Type,
                requestBody,
                paymentIntentId,
                mandateId,
                chargeId,
                status,
                FailureClassifier.Classify(status, failureCode),
                failureCode,
                failureMessage);
        }

        private static ProviderPaymentMethodResult MapPaymentMethod(PaymentMethodKind kind, PaymentMethod pm, string verificationStatus)
        {
            if (kind == PaymentMethodKind.Card)
            {
                return new ProviderPaymentMethodResult(
                    pm.Id,
                    PaymentMethodKind.Card,
                    pm.Card?.Brand,
                    pm.Card?.Last4,
                    (int?)pm.Card?.ExpMonth,
                    (int?)pm.Card?.ExpYear,
                    null,
                    verificationStatus,
                    pm.Card?.Funding);
            }

            return new ProviderPaymentMethodResult(
                pm.Id,
                PaymentMethodKind.Pad,
                null,
                pm.AcssDebit?.Last4,
                null,
                null,
                pm.AcssDebit?.BankName,
                verificationStatus,
                null);
        }
    }
}
