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
    public class RatingController : ControllerBase
    {
        // GET: api/<RatingController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllRatings")]
        public async Task<IActionResult> Get([FromServices] IRatingService ratingService, [FromQuery] Paging paging)
        {
            return Ok(await ratingService.GetAll(paging));
        }

        // GET api/<RatingController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetRatingByID")]
        public async Task<IActionResult> GetRatingByID([FromServices] IRatingService ratingService, Guid id)
        {
            var result = await ratingService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Rating with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<RatingController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateRating")]
        public async Task<IActionResult> CreateRating([FromServices] IRatingService ratingService, [FromBody] RatingDto ratingDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await ratingService.CreateRating(ratingDto);
            if (result.RatingID != null && result.RatingID != Guid.Empty)
                return CreatedAtRoute("GetRatingByID", new { id = result.RatingID }, result);
            return BadRequest(new { message = "Failed to create rating. A rating with the same name may already exist." });
        }

        // PUT api/<RatingController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateRating")]
        public async Task<IActionResult> UpdateRating([FromServices] IRatingService ratingService, Guid id, [FromBody] RatingDto ratingDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await ratingService.UpdateRating(id, ratingDto);
            if (result == null)
                return NotFound(new { message = "Rating with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<RatingController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteRating")]
        public async Task<IActionResult> DeleteRating([FromServices] IRatingService ratingService, Guid id)
        {
            try
            {
                await ratingService.DeleteRating(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Rating with the specified ID was not found." });
            }
        }
    }
}