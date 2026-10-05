using Arcora.Api.DTOs;
using Arcora.Api.Exceptions;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arcora.Api.Controllers;

[ApiController]
[Route("api/leases/{leaseId:guid}/check-in")]
[Authorize(Roles = "User, LandLord, Admin")]
public class LeaseCheckInController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(LeaseCheckInResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLeaseCheckIn([FromServices] ILeaseCheckInService leaseCheckInService, Guid leaseId, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await leaseCheckInService.GetAsync(leaseId, User, cancellationToken));
        }
        catch (LeaseCheckInException ex)
        {
            return ToProblem(ex);
        }
    }

    [HttpPost("draft")]
    [ProducesResponseType(typeof(LeaseCheckInDraftResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(LeaseCheckInDraftResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateOrGetDraft([FromServices] ILeaseCheckInService leaseCheckInService, Guid leaseId, CancellationToken cancellationToken)
    {
        try
        {
            var (draft, created) = await leaseCheckInService.CreateOrGetDraftAsync(leaseId, User, cancellationToken);
            if (created)
            {
                return StatusCode(StatusCodes.Status201Created, draft);
            }

            return Ok(draft);
        }
        catch (LeaseCheckInException ex)
        {
            return ToProblem(ex);
        }
    }

    [HttpPost("photos")]
    [RequestFormLimits(MultipartBodyLengthLimit = 10 * 1024 * 1024)]
    [RequestSizeLimit(10 * 1024 * 1024)]
    [ProducesResponseType(typeof(LeaseCheckInPhotoUploadResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(LeaseCheckInPhotoUploadResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> UploadPhoto([FromServices] ILeaseCheckInService leaseCheckInService, Guid leaseId, [FromForm] LeaseCheckInPhotoUploadRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await leaseCheckInService.UploadPhotoAsync(leaseId, request, User, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (LeaseCheckInException ex)
        {
            return ToProblem(ex);
        }
    }

    [HttpDelete("photos/{attachmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> DeletePhoto([FromServices] ILeaseCheckInService leaseCheckInService, Guid leaseId, Guid attachmentId, CancellationToken cancellationToken)
    {
        try
        {
            await leaseCheckInService.DeletePhotoAsync(leaseId, attachmentId, User, cancellationToken);
            return NoContent();
        }
        catch (LeaseCheckInException ex)
        {
            return ToProblem(ex);
        }
    }

    [HttpPost("submit")]
    [ProducesResponseType(typeof(LeaseCheckInInspectionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Submit([FromServices] ILeaseCheckInService leaseCheckInService, Guid leaseId, [FromBody] SubmitLeaseCheckInRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var idempotencyKey = Request.Headers["Idempotency-Key"].ToString();
            var response = await leaseCheckInService.SubmitAsync(leaseId, request, idempotencyKey, User, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (LeaseCheckInException ex)
        {
            return ToProblem(ex);
        }
    }

    private ObjectResult ToProblem(LeaseCheckInException ex)
    {
        var problem = new ProblemDetails
        {
            Status = ex.StatusCode,
            Title = ex.Title,
            Detail = ex.Detail,
            Instance = HttpContext.Request.Path
        };

        problem.Extensions["correlationId"] = HttpContext.TraceIdentifier;
        if (!string.IsNullOrWhiteSpace(ex.ReasonCode))
        {
            problem.Extensions["reasonCode"] = ex.ReasonCode;
        }

        if (ex.Errors != null && ex.Errors.Count > 0)
        {
            problem.Extensions["errors"] = ex.Errors;
        }

        return StatusCode(ex.StatusCode, problem);
    }
}

[ApiController]
[Route("api/inspections/{inspectionId:guid}/move-in/reviews")]
[Authorize(Roles = "LandLord, Admin")]
public class InspectionMoveInReviewController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(InspectionReviewRecordDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateReview([FromServices] ILeaseCheckInService leaseCheckInService, Guid inspectionId, [FromBody] InspectionReviewRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await leaseCheckInService.CreateReviewAsync(inspectionId, request, User, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (LeaseCheckInException ex)
        {
            var problem = new ProblemDetails
            {
                Status = ex.StatusCode,
                Title = ex.Title,
                Detail = ex.Detail,
                Instance = HttpContext.Request.Path
            };
            problem.Extensions["correlationId"] = HttpContext.TraceIdentifier;
            if (!string.IsNullOrWhiteSpace(ex.ReasonCode))
            {
                problem.Extensions["reasonCode"] = ex.ReasonCode;
            }
            if (ex.Errors != null && ex.Errors.Count > 0)
            {
                problem.Extensions["errors"] = ex.Errors;
            }
            return StatusCode(ex.StatusCode, problem);
        }
    }
}
