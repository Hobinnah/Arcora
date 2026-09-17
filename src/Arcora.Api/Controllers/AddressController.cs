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

namespace Arcora.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
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
        public async Task<IActionResult> CreateAddress([FromServices] IAddressService addressService, [FromBody] AddressDto addressDto)
        {
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
        public async Task<IActionResult> UpdateAddress([FromServices] IAddressService addressService, Guid id, [FromBody] AddressDto addressDto)
        {
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
