using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Payments.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arcora.Api.Controllers
{
    /// <summary>
    /// Receives Stripe webhook callbacks. The endpoint is anonymous (Stripe cannot authenticate) but is
    /// protected by mandatory signature verification. Events are persisted idempotently before being
    /// dispatched to the rent-collection orchestrator so the definitive PAD/card outcome can drive the
    /// state machine.
    /// </summary>
    [Route("api/webhooks/stripe")]
    [ApiController]
    [AllowAnonymous]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IPaymentProvider paymentProvider;
        private readonly IPaymentProviderEventRepository eventRepository;
        private readonly IPaymentIntentRepository paymentIntentRepository;
        private readonly IRentCollectionOrchestrator orchestrator;
        private readonly IPaymentOnboardingService paymentOnboardingService;
        private readonly ILogger<StripeWebhookController> logger;

        public StripeWebhookController(
            IPaymentProvider paymentProvider,
            IPaymentProviderEventRepository eventRepository,
            IPaymentIntentRepository paymentIntentRepository,
            IRentCollectionOrchestrator orchestrator,
            IPaymentOnboardingService paymentOnboardingService,
            ILogger<StripeWebhookController> logger)
        {
            this.paymentProvider = paymentProvider;
            this.eventRepository = eventRepository;
            this.paymentIntentRepository = paymentIntentRepository;
            this.orchestrator = orchestrator;
            this.paymentOnboardingService = paymentOnboardingService;
            this.logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Handle()
        {
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"].ToString();

            ProviderWebhookEvent webhookEvent;
            try
            {
                webhookEvent = paymentProvider.ConstructWebhookEvent(payload, signature);
            }
            catch (Stripe.StripeException ex)
            {
                // A StripeException here is either an invalid signature or a payload parse/API version
                // mismatch. Both are client-visible 400s but are logged distinctly for diagnosis.
                logger.LogWarning(ex, "Rejected Stripe webhook: {Message}", ex.Message);
                return BadRequest(new { message = "Unable to process webhook payload." });
            }

            // Idempotency: ignore events we have already stored.
            var existing = await eventRepository.Find(x => x.ProviderEventID == webhookEvent.EventId);
            if (existing != null && existing.Any())
            {
                return Ok(new { received = true, duplicate = true });
            }

            var providerEvent = new PaymentProviderEvent
            {
                PaymentProviderEventID = Guid.NewGuid(),
                ProviderName = paymentProvider.ProviderName,
                ProviderEventID = webhookEvent.EventId,
                EventType = webhookEvent.EventType,
                ProcessingStatus = "RECEIVED",
                Payload = webhookEvent.RawPayload,
                ReceivedAt = DateTime.UtcNow,
                CapturedDate = DateTime.UtcNow
            };
            await eventRepository.Create(providerEvent);
            await eventRepository.Save();

            try
            {
                await DispatchAsync(webhookEvent);
                providerEvent.ProcessingStatus = "PROCESSED";
                providerEvent.ProcessedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process Stripe event {EventId}", webhookEvent.EventId);
                providerEvent.ProcessingStatus = "FAILED";
                providerEvent.FailureReason = ex.Message;
            }

            await eventRepository.Update(providerEvent);
            await eventRepository.Save();

            return Ok(new { received = true });
        }

        private async Task DispatchAsync(ProviderWebhookEvent webhookEvent)
        {
            switch (webhookEvent.EventType)
            {
                case "payment_intent.succeeded":
                case "payment_intent.payment_failed":
                case "charge.succeeded":
                case "charge.failed":
                    var localIntent = await ResolveLocalIntentAsync(webhookEvent.PaymentIntentId);
                    if (localIntent != null)
                    {
                        await orchestrator.HandlePadResultAsync(
                            localIntent.PaymentIntentID,
                            webhookEvent.Status ?? string.Empty,
                            webhookEvent.FailureCode,
                            webhookEvent.FailureMessage);
                    }
                    else
                    {
                        logger.LogWarning("No local PaymentIntent for provider id {ProviderId}", webhookEvent.PaymentIntentId);
                    }
                    break;

                // Verification outcomes for saved PAD/card methods. These transition a stored payment
                // method from PENDING to VERIFIED/FAILED and activate the PAD autopay mandate.
                case "setup_intent.succeeded":
                case "setup_intent.setup_failed":
                case "setup_intent.canceled":
                case "mandate.updated":
                    await paymentOnboardingService.UpdateVerificationStatusAsync(
                        webhookEvent.PaymentMethodId,
                        webhookEvent.Status);
                    break;

                default:
                    logger.LogInformation("Unhandled Stripe event type {Type}", webhookEvent.EventType);
                    break;
            }
        }

        private async Task<PaymentIntent?> ResolveLocalIntentAsync(string? providerPaymentIntentId)
        {
            if (string.IsNullOrWhiteSpace(providerPaymentIntentId))
                return null;
            var matches = await paymentIntentRepository.Find(x => x.ProviderPaymentIntentID == providerPaymentIntentId);
            return matches?.FirstOrDefault();
        }
    }
}
