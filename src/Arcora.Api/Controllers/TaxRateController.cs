// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arcora.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class TaxRateController : ControllerBase
    {
        // GET: api/<TaxRateController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllTaxRates")]
        public async Task<IActionResult> Get([FromServices] ITaxRateService taxrateService, [FromQuery] Paging paging)
        {
            return Ok(await taxrateService.GetAll(paging));
        }

        // GET api/<TaxRateController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetTaxRateByID")]
        public async Task<IActionResult> GetTaxRateByID([FromServices] ITaxRateService taxrateService, int id)
        {
            var result = await taxrateService.GetID(id);
            if (result == null)
                return NotFound(new { message = "TaxRate with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<TaxRateController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateTaxRate")]
        public async Task<IActionResult> CreateTaxRate([FromServices] ITaxRateService taxrateService, [FromBody] TaxRateDto taxrateDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await taxrateService.CreateTaxRate(taxrateDto);
            if (result != null && result.TaxID != 0)
                return CreatedAtRoute("GetTaxRateByID", new { id = result.TaxID }, result);
            return BadRequest(new { message = "Failed to create taxrate. A taxrate with the same name may already exist." });
        }

        // PUT api/<TaxRateController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateTaxRate")]
        public async Task<IActionResult> UpdateTaxRate([FromServices] ITaxRateService taxrateService, int id, [FromBody] TaxRateDto taxrateDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await taxrateService.UpdateTaxRate(id, taxrateDto);
            if (result == null)
                return NotFound(new { message = "TaxRate with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<TaxRateController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteTaxRate")]
        public async Task<IActionResult> DeleteTaxRate([FromServices] ITaxRateService taxrateService, int id)
        {
            try
            {
                await taxrateService.DeleteTaxRate(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "TaxRate with the specified ID was not found." });
            }
        }
    }
}