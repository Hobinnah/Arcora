// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Arcora.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ListingController : ControllerBase
    {
        // GET: api/<ListingController>
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllListings")]
        public async Task<IActionResult> Get([FromServices] IListingService listingService, [FromQuery] Paging paging)
        {
            return Ok(await listingService.GetAll(paging));
        }

        // POST api/<ListingController>/Import
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "PublishListing")]
        public async Task<IActionResult> PublishListing([FromServices] IListingService listingService, [FromServices] IOrganizationMemberRepository organizationMemberRepository, [FromBody] ListingImportDto payload)
        {
            if (payload == null)
                return BadRequest(new { message = "Payload is required." });
            if (!User.IsInRole("Admin"))
            {
                var accessDenied = await RequireOrganizationAccessAsync(payload.Property?.OrganizationID, organizationMemberRepository);
                if (accessDenied != null)
                    return accessDenied;
            }
            var result = await listingService.ImportListingPayload(payload);
            return Ok(result);
        }

        // GET: api/<ListingController>/Search
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "SearchListings")]
        public async Task<IActionResult> Search([FromServices] IListingService listingService, [FromQuery] ListingSearchCriteria criteria)
        {
            return Ok(await listingService.SearchListings(criteria));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> MapSearch([FromServices] IListingRepository repository,
            [FromQuery] ListingSearchCriteria criteria, CancellationToken cancellationToken)
        {
            return Ok(await repository.SearchMapAsync(criteria, cancellationToken));
        }

        // GET: api/<ListingController>/GetByOrganization/{organizationId}
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{organizationId}", Name = "GetListingsByOrganization")]
        public async Task<IActionResult> GetByOrganization([FromServices] IListingService listingService, [FromServices] IOrganizationMemberRepository organizationMemberRepository, Guid organizationId, [FromQuery] Paging paging)
        {
            var accessDenied = await RequireOrganizationAccessAsync(organizationId, organizationMemberRepository);
            if (accessDenied != null)
                return accessDenied;

            return Ok(await listingService.GetListingsByOrganization(organizationId, paging));
        }

        // GET: api/<ListingController>/CountByOrganization/{organizationId}
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{organizationId}", Name = "CountListingsByOrganization")]
        public async Task<IActionResult> CountByOrganization([FromServices] IListingService listingService, [FromServices] IOrganizationMemberRepository organizationMemberRepository, Guid organizationId)
        {
            var accessDenied = await RequireOrganizationAccessAsync(organizationId, organizationMemberRepository);
            if (accessDenied != null)
                return accessDenied;

            return Ok(await listingService.GetListingsCountByOrganization(organizationId));
        }

        // GET api/<ListingController>/5
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetListingByID")]
        public async Task<IActionResult> GetListingByID([FromServices] IListingService listingService, [FromServices] IOrganizationMemberRepository organizationMemberRepository, Guid id)
        {
            var result = await listingService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Listing with the specified ID was not found." });

            if (!IsMarketplaceVisible(result.Status))
            {
                var accessDenied = await RequireOrganizationAccessAsync(result.OrganizationID, organizationMemberRepository);
                if (accessDenied != null)
                    return NotFound(new { message = "Listing with the specified ID was not found." });
            }

            return Ok(result);
        }

        // POST api/<ListingController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateListing")]
        public async Task<IActionResult> CreateListing([FromServices] IListingService listingService, [FromServices] IOrganizationMemberRepository organizationMemberRepository, [FromBody] ListingDto listingDto)
        {
            if (!User.IsInRole("Admin"))
            {
                var accessDenied = await RequireOrganizationAccessAsync(listingDto.OrganizationID, organizationMemberRepository);
                if (accessDenied != null)
                    return accessDenied;
                listingDto.Status = "DRAFT";
                listingDto.PublishedAt = null;
                listingDto.ReviewSubmittedAt = null;
                listingDto.ReviewedAt = null;
                listingDto.ReviewedByUserID = null;
                listingDto.ReviewMessage = null;
            }
            var result = await listingService.CreateListing(listingDto);
            if (result.ListingID != null && result.ListingID != Guid.Empty)
                return CreatedAtRoute("GetListingByID", new { id = result.ListingID }, result);
            return BadRequest(new { message = "Failed to create listing. A listing with the same name may already exist." });
        }

        // PUT api/<ListingController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateListing")]
        public async Task<IActionResult> UpdateListing([FromServices] IListingService listingService, [FromServices] IOrganizationMemberRepository organizationMemberRepository, Guid id, [FromBody] ListingDto listingDto)
        {
            var existing = await listingService.GetID(id);
            if (existing == null)
                return NotFound(new { message = "Listing with the specified ID was not found." });

            if (!User.IsInRole("Admin"))
            {
                var accessDenied = await RequireOrganizationAccessAsync(existing.OrganizationID, organizationMemberRepository);
                if (accessDenied != null)
                    return accessDenied;
                if (listingDto.OrganizationID != existing.OrganizationID)
                    return Forbid();
                if (!string.Equals(listingDto.Status, existing.Status, StringComparison.OrdinalIgnoreCase))
                    return Forbid();
                listingDto.ReviewSubmittedAt = existing.ReviewSubmittedAt;
                listingDto.ReviewedAt = existing.ReviewedAt;
                listingDto.ReviewedByUserID = existing.ReviewedByUserID;
                listingDto.ReviewMessage = existing.ReviewMessage;
                listingDto.PublishedAt = existing.PublishedAt;
            }
            var result = await listingService.UpdateListing(id, listingDto);
            if (result == null)
                return NotFound(new { message = "Listing with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<ListingController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteListing")]
        public async Task<IActionResult> DeleteListing([FromServices] IListingService listingService, Guid id)
        {
            try
            {
                await listingService.DeleteListing(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Listing with the specified ID was not found." });
            }
        }

        // POST: api/listing/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateListingStatus")]
        public async Task<IActionResult> UpdateListingStatus([FromServices] IListingService listingService, [FromServices] IOrganizationMemberRepository organizationMemberRepository, [FromRoute] Guid id, [FromRoute] string status)
        {
            if (IsReviewControlledStatus(status) && !User.IsInRole("Admin"))
                return Forbid();

            var existing = await listingService.GetID(id);
            if (existing == null)
                return NotFound($"Listing with ID {id} not found.");

            var accessDenied = await RequireOrganizationAccessAsync(existing.OrganizationID, organizationMemberRepository);
            if (accessDenied != null)
                return accessDenied;

            var listing = await listingService.UpdateListingStatus(id, status);
            if (listing == null)
                return NotFound($"Listing with ID {id} not found.");
            return Ok(listing);
        }

        // GET /api/listing/available?organizationId=...&startDate=YYYY-MM-DD&leaseTermMonths=6
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("/api/listing/available", Name = "GetAvailableListingsForRequestedTerm")]
        public async Task<IActionResult> GetAvailable([FromServices] IListingService listingService, [FromServices] IOrganizationMemberRepository organizationMemberRepository, [FromQuery] Guid organizationId, [FromQuery] DateTime startDate, [FromQuery] short leaseTermMonths)
        {
            if (organizationId == Guid.Empty || leaseTermMonths <= 0)
                return BadRequest(new { message = "organizationId and a positive leaseTermMonths are required." });

            var accessDenied = await RequireOrganizationAccessAsync(organizationId, organizationMemberRepository);
            if (accessDenied != null)
                return accessDenied;

            var result = await listingService.GetAvailableListings(organizationId, startDate, leaseTermMonths);
            return Ok(result);
        }

        // GET /api/listing/{listingId}/availability?organizationId=...&startDate=YYYY-MM-DD&leaseTermMonths=6
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("/api/listing/{listingId}/availability", Name = "GetListingAvailabilityForRequestedTerm")]
        public async Task<IActionResult> GetAvailability([FromServices] IListingService listingService, [FromServices] IOrganizationMemberRepository organizationMemberRepository, [FromRoute] Guid listingId, [FromQuery] Guid organizationId, [FromQuery] DateTime startDate, [FromQuery] short leaseTermMonths)
        {
            if (listingId == Guid.Empty || organizationId == Guid.Empty || leaseTermMonths <= 0)
                return BadRequest(new { message = "listingId, organizationId and a positive leaseTermMonths are required." });

            var accessDenied = await RequireOrganizationAccessAsync(organizationId, organizationMemberRepository);
            if (accessDenied != null)
                return accessDenied;

            var result = await listingService.GetListingAvailability(organizationId, listingId, startDate, leaseTermMonths);
            if (result == null)
                return NotFound(new { message = "Listing was not found for the provided organization." });

            return Ok(result);
        }

        private async Task<IActionResult?> RequireOrganizationAccessAsync(Guid? organizationID, IOrganizationMemberRepository organizationMemberRepository)
        {
            if (!organizationID.HasValue || organizationID.Value == Guid.Empty)
                return NotFound();

            if (User.IsInRole("Admin"))
                return null;

            var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(userIdClaim, out var userId) || userId <= 0)
                return Unauthorized();

            var memberships = await organizationMemberRepository.GetMemberOrganizationsAsync(userId);
            return memberships.Any(member =>
                    member.OrganizationID == organizationID.Value &&
                    string.Equals(member.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                ? null
                : Forbid();
        }

        private static bool IsMarketplaceVisible(string? status)
        {
            return string.Equals(status, "APPROVED", StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, "PUBLISHED", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsReviewControlledStatus(string? status)
        {
            return IsMarketplaceVisible(status)
                || string.Equals(status, "PENDING_REVIEW", StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, "REJECTED", StringComparison.OrdinalIgnoreCase);
        }
    }
}