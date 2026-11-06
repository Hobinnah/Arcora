// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Arcora.Api.DTOs;
using Microsoft.Extensions.Logging;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Arcora.Api.Services.Implementations;
using Microsoft.AspNetCore.RateLimiting;

namespace Arcora.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost]
        [EnableRateLimiting("MapsPolicy")]
        public async Task<IActionResult> Geocode([FromServices] AzureMapsService maps,
            [FromServices] PropertyLocationAccess access,
            [FromServices] ILogger<AddressController> logger, [FromBody] AddressDto address, CancellationToken ct)
        {
            if (!await access.HasHostMembershipAsync(User, ct)) return Forbid();
            if (!maps.CanGeocode)
                return StatusCode(503, new { message = "Address geocoding is not configured. Confirm the coordinates manually." });
            try
            {
                var result = await maps.GeocodeAsync(address, ct);
                return result == null ? UnprocessableEntity(new { message = "A street-level location could not be verified. Check the address or confirm the coordinates manually." }) : Ok(result);
            }
            catch (HttpRequestException error)
            {
                logger.LogWarning("Azure Maps geocoding failed with HTTP status {Status}.", error.StatusCode);
                return StatusCode(503, new { message = "Address geocoding is unavailable. Try again or confirm the coordinates manually." });
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                logger.LogWarning("Azure Maps geocoding timed out.");
                return StatusCode(503, new { message = "Address geocoding timed out. Try again shortly." });
            }
            catch (Azure.Identity.AuthenticationFailedException)
            {
                logger.LogWarning("Azure Maps geocoding authentication failed.");
                return StatusCode(503, new { message = "Address geocoding authentication is unavailable." });
            }
            catch (JsonException)
            {
                logger.LogWarning("Azure Maps returned an invalid geocoding response.");
                return StatusCode(503, new { message = "Address geocoding returned an invalid response. Try again shortly." });
            }
        }
        // GET: api/<AddressController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllAddresses")]
        public async Task<IActionResult> Get([FromServices] IAddressService addressService, [FromQuery] Paging paging)
        {
            return Ok(await addressService.GetAll(paging));
        }

        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [HttpGet(Name = "LookupPostalCode")]
        public async Task<IActionResult> LookupPostalCode([FromServices] IAddressService addressService, [FromQuery] string postalCode, [FromQuery] string country = "CAN")
        {
            if (string.IsNullOrWhiteSpace(postalCode))
                return BadRequest(new { message = "postalCode is required." });

            // Delegate to AddressService implementation
            var serviceSuggestions = await addressService.LookupAddressesByPostalCode(postalCode, country);
            return Ok(serviceSuggestions);           
        }

        // POST api/<AddressController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateAddress")]
        public async Task<IActionResult> CreateAddress([FromServices] IAddressService addressService,
            [FromServices] PropertyLocationAccess access, [FromBody] AddressDto addressDto)
        {
            if (!await access.CanManageOrganizationAsync(User, addressDto.OrganizationID, HttpContext.RequestAborted))
                return Forbid();
            var result = await addressService.CreateAddress(addressDto);
            if (result.AddressID != null && result.AddressID != Guid.Empty)
                return CreatedAtRoute("GetAddressByID", new { id = result.AddressID }, result);
            return BadRequest(new { message = "Failed to create address. A address with the same name may already exist." });
        }

        // PUT api/<AddressController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateAddress")]
        public async Task<IActionResult> UpdateAddress([FromServices] IAddressService addressService,
            [FromServices] PropertyLocationAccess access, Guid id, [FromBody] AddressDto addressDto)
        {
            var existing = await addressService.GetID(id);
            if (existing == null) return NotFound(new { message = "Address with the specified ID was not found." });
            if (!await access.CanManageAddressAsync(User, id, HttpContext.RequestAborted) ||
                existing.OrganizationID != addressDto.OrganizationID)
                return Forbid();
            var result = await addressService.UpdateAddress(id, addressDto);
            if (result == null)
                return NotFound(new { message = "Address with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<AddressController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteAddress")]
        public async Task<IActionResult> DeleteAddress([FromServices] IAddressService addressService, Guid id)
        {
            try
            {
                await addressService.DeleteAddress(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Address with the specified ID was not found." });
            }
        }
    }
}
