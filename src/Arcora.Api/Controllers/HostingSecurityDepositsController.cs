using Arcora.Api.DTOs;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Arcora.Api.Controllers
{
    [ApiController]
    [Route("api/hosting/securitydeposits")]
    [Authorize(Roles = "User, LandLord, Admin")]
    public class HostingSecurityDepositsController : ControllerBase
    {
        private const long MaxEvidenceSizeBytes = 10 * 1024 * 1024;

        private readonly IHostingSecurityDepositService hostingSecurityDepositService;

        public HostingSecurityDepositsController(IHostingSecurityDepositService hostingSecurityDepositService)
        {
            this.hostingSecurityDepositService = hostingSecurityDepositService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHostSecurityDeposits([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1, [FromQuery] string? status = null)
        {
            var actorUserID = ResolveActorUserID();
            var response = await hostingSecurityDepositService.GetHostSecurityDeposits(actorUserID, pageSize, pageNumber, status);
            return Ok(response);
        }

        [HttpGet("/api/securitydeposit/{securityDepositID:guid}/transactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTransactionsBySecurityDeposit([FromRoute] Guid securityDepositID, [FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
        {
            var actorUserID = ResolveActorUserID();
            var response = await hostingSecurityDepositService.GetTransactionsBySecurityDeposit(securityDepositID, actorUserID, pageSize, pageNumber);
            return Ok(response);
        }

        [HttpPost("{securityDepositID:guid}/evidence")]
        [RequestSizeLimit(MaxEvidenceSizeBytes)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadEvidence([FromRoute] Guid securityDepositID, [FromForm] SecurityDepositEvidenceUploadRequestDto request, CancellationToken cancellationToken)
        {
            if (request?.File == null)
                return BadRequest(new { message = "No file was provided." });

            var actorUserID = ResolveActorUserID();
            var response = await hostingSecurityDepositService.UploadEvidence(securityDepositID, actorUserID, request.File, cancellationToken);
            return Created(string.Empty, response);
        }

        [HttpPost("/api/securitydeposit/{securityDepositID:guid}/settlement-notice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendSettlementNotice([FromRoute] Guid securityDepositID, [FromBody] SecurityDepositSettlementNoticeRequestDto request)
        {
            var actorUserID = ResolveActorUserID();
            var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
            var response = await hostingSecurityDepositService.SendSettlementNotice(securityDepositID, actorUserID, idempotencyKey, request);
            return Ok(response);
        }

        [HttpPost("/api/securitydeposit/{securityDepositID:guid}/return")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReturnDeposit([FromRoute] Guid securityDepositID, [FromBody] SecurityDepositReturnRequestDto request)
        {
            var actorUserID = ResolveActorUserID();
            var response = await hostingSecurityDepositService.ReturnDeposit(securityDepositID, actorUserID, request);
            return Ok(response);
        }

        private long ResolveActorUserID()
        {
            var claimValue = User.FindFirst("UserId")?.Value;
            if (long.TryParse(claimValue, out var userID) && userID > 0)
                return userID;

            claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (long.TryParse(claimValue, out userID) && userID > 0)
                return userID;

            throw new UnauthorizedAccessException("Authenticated user context is invalid.");
        }

    }
}
