using Arcora.Api.DTOs;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arcora.Api.Controllers
{
    /// <summary>
    /// Tenant payment onboarding: provision the provider customer, save the primary PAD (with ACSS
    /// mandate) and the mandatory backup credit card, and report readiness. Raw instrument data is
    /// collected client-side by the provider SDK - only opaque tokens reach these endpoints.
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class PaymentOnboardingController : ControllerBase
    {
        // POST: api/PaymentOnboarding/Start
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost(Name = "StartPaymentOnboarding")]
        public async Task<IActionResult> Start([FromServices] IPaymentOnboardingService onboardingService, [FromBody] PaymentOnboardingStartRequest request)
        {
            try
            {
                var result = await onboardingService.StartAsync(request.TenantID);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/PaymentOnboarding/CreateSetupIntent
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost(Name = "CreateSetupIntent")]
        public async Task<IActionResult> CreateSetupIntent([FromServices] IPaymentOnboardingService onboardingService, [FromBody] CreateSetupIntentRequest request)
        {
            try
            {
                var result = await onboardingService.CreateSetupIntentAsync(request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex) when (ex is ArgumentException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/PaymentOnboarding/SavePaymentMethod
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost(Name = "SavePaymentMethod")]
        public async Task<IActionResult> SavePaymentMethod([FromServices] IPaymentOnboardingService onboardingService, [FromBody] SavePaymentMethodRequest request)
        {
            try
            {
                var result = await onboardingService.SavePaymentMethodAsync(request);
                return CreatedAtRoute("GetPaymentMethodByID", new { id = result.PaymentMethodID }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/PaymentOnboarding/Status/{tenantId}
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{tenantId}", Name = "GetPaymentOnboardingStatus")]
        public async Task<IActionResult> Status([FromServices] IPaymentOnboardingService onboardingService, Guid tenantId)
        {
            var result = await onboardingService.GetStatusAsync(tenantId);
            return Ok(result);
        }
    }
}
