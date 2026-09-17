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
    public class MaintenanceRequestController : ControllerBase
    {
        // GET: api/<MaintenanceRequestController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllMaintenanceRequests")]
        public async Task<IActionResult> Get([FromServices] IMaintenanceRequestService maintenancerequestService, [FromQuery] Paging paging)
        {
            return Ok(await maintenancerequestService.GetAll(paging));
        }

        // GET api/<MaintenanceRequestController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetMaintenanceRequestByID")]
        public async Task<IActionResult> GetMaintenanceRequestByID([FromServices] IMaintenanceRequestService maintenancerequestService, Guid id)
        {
            var result = await maintenancerequestService.GetID(id);
            if (result == null)
                return NotFound(new { message = "MaintenanceRequest with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<MaintenanceRequestController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateMaintenanceRequest")]
        public async Task<IActionResult> CreateMaintenanceRequest([FromServices] IMaintenanceRequestService maintenancerequestService, [FromBody] MaintenanceRequestDto maintenancerequestDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await maintenancerequestService.CreateMaintenanceRequest(maintenancerequestDto);
            if (result.MaintenanceRequestID != null && result.MaintenanceRequestID != Guid.Empty)
                return CreatedAtRoute("GetMaintenanceRequestByID", new { id = result.MaintenanceRequestID }, result);
            return BadRequest(new { message = "Failed to create maintenancerequest. A maintenancerequest with the same name may already exist." });
        }

        // PUT api/<MaintenanceRequestController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateMaintenanceRequest")]
        public async Task<IActionResult> UpdateMaintenanceRequest([FromServices] IMaintenanceRequestService maintenancerequestService, Guid id, [FromBody] MaintenanceRequestDto maintenancerequestDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await maintenancerequestService.UpdateMaintenanceRequest(id, maintenancerequestDto);
            if (result == null)
                return NotFound(new { message = "MaintenanceRequest with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<MaintenanceRequestController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteMaintenanceRequest")]
        public async Task<IActionResult> DeleteMaintenanceRequest([FromServices] IMaintenanceRequestService maintenancerequestService, Guid id)
        {
            try
            {
                await maintenancerequestService.DeleteMaintenanceRequest(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "MaintenanceRequest with the specified ID was not found." });
            }
        }

        // POST: api/maintenancerequest/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateMaintenanceRequestStatus")]
        public async Task<ActionResult> UpdateMaintenanceRequestStatus([FromServices] IMaintenanceRequestService maintenancerequestService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var maintenancerequest = await maintenancerequestService.UpdateMaintenanceRequestStatus(id, status);
            if (maintenancerequest == null)
                return NotFound($"MaintenanceRequest with ID {id} not found.");
            return Ok(maintenancerequest);
        }
    }
}