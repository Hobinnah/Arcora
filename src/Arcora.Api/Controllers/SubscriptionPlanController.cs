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
    public class SubscriptionPlanController : ControllerBase
    {
        // GET: api/<SubscriptionPlanController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllSubscriptionPlans")]
        public async Task<IActionResult> Get([FromServices] ISubscriptionPlanService subscriptionplanService, [FromQuery] Paging paging)
        {
            return Ok(await subscriptionplanService.GetAll(paging));
        }

        // GET api/<SubscriptionPlanController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetSubscriptionPlanByID")]
        public async Task<IActionResult> GetSubscriptionPlanByID([FromServices] ISubscriptionPlanService subscriptionplanService, Guid id)
        {
            var result = await subscriptionplanService.GetID(id);
            if (result == null)
                return NotFound(new { message = "SubscriptionPlan with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<SubscriptionPlanController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateSubscriptionPlan")]
        public async Task<IActionResult> CreateSubscriptionPlan([FromServices] ISubscriptionPlanService subscriptionplanService, [FromBody] SubscriptionPlanDto subscriptionplanDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await subscriptionplanService.CreateSubscriptionPlan(subscriptionplanDto);
            if (result.SubscriptionPlanID != null && result.SubscriptionPlanID != Guid.Empty)
                return CreatedAtRoute("GetSubscriptionPlanByID", new { id = result.SubscriptionPlanID }, result);
            return BadRequest(new { message = "Failed to create subscriptionplan. A subscriptionplan with the same name may already exist." });
        }

        // PUT api/<SubscriptionPlanController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateSubscriptionPlan")]
        public async Task<IActionResult> UpdateSubscriptionPlan([FromServices] ISubscriptionPlanService subscriptionplanService, Guid id, [FromBody] SubscriptionPlanDto subscriptionplanDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await subscriptionplanService.UpdateSubscriptionPlan(id, subscriptionplanDto);
            if (result == null)
                return NotFound(new { message = "SubscriptionPlan with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<SubscriptionPlanController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteSubscriptionPlan")]
        public async Task<IActionResult> DeleteSubscriptionPlan([FromServices] ISubscriptionPlanService subscriptionplanService, Guid id)
        {
            try
            {
                await subscriptionplanService.DeleteSubscriptionPlan(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "SubscriptionPlan with the specified ID was not found." });
            }
        }
    }
}