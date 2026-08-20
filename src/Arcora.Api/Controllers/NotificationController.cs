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
    public class NotificationController : ControllerBase
    {
        // GET: api/<NotificationController>
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllNotifications")]
        public async Task<IActionResult> Get([FromServices] INotificationService notificationService, [FromQuery] Paging paging)
        {
            return Ok(await notificationService.GetAll(paging));
        }

        // GET api/<NotificationController>/5
        [Authorize(Roles = "Viewer, User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetNotificationByID")]
        public async Task<IActionResult> GetNotificationByID([FromServices] INotificationService notificationService, Guid id)
        {
            var result = await notificationService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Notification with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<NotificationController>
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateNotification")]
        public async Task<IActionResult> CreateNotification([FromServices] INotificationService notificationService, [FromBody] NotificationDto notificationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await notificationService.CreateNotification(notificationDto);
            if (result.NotificationID != null && result.NotificationID != Guid.Empty)
                return CreatedAtRoute("GetNotificationByID", new { id = result.NotificationID }, result);
            return BadRequest(new { message = "Failed to create notification. A notification with the same name may already exist." });
        }

        // PUT api/<NotificationController>/5
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateNotification")]
        public async Task<IActionResult> UpdateNotification([FromServices] INotificationService notificationService, Guid id, [FromBody] NotificationDto notificationDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await notificationService.UpdateNotification(id, notificationDto);
            if (result == null)
                return NotFound(new { message = "Notification with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<NotificationController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteNotification")]
        public async Task<IActionResult> DeleteNotification([FromServices] INotificationService notificationService, Guid id)
        {
            try
            {
                await notificationService.DeleteNotification(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Notification with the specified ID was not found." });
            }
        }

        // POST: api/notification/{id}/{status}
        [Authorize(Roles = "User, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateNotificationStatus")]
        public async Task<ActionResult> UpdateNotificationStatus([FromServices] INotificationService notificationService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var notification = await notificationService.UpdateNotificationStatus(id, status);
            if (notification == null)
                return NotFound($"Notification with ID {id} not found.");
            return Ok(notification);
        }
    }
}