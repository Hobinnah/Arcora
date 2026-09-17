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
    public class WorkOrderController : ControllerBase
    {
        // GET: api/<WorkOrderController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllWorkOrders")]
        public async Task<IActionResult> Get([FromServices] IWorkOrderService workorderService, [FromQuery] Paging paging)
        {
            return Ok(await workorderService.GetAll(paging));
        }

        // GET api/<WorkOrderController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetWorkOrderByID")]
        public async Task<IActionResult> GetWorkOrderByID([FromServices] IWorkOrderService workorderService, Guid id)
        {
            var result = await workorderService.GetID(id);
            if (result == null)
                return NotFound(new { message = "WorkOrder with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<WorkOrderController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateWorkOrder")]
        public async Task<IActionResult> CreateWorkOrder([FromServices] IWorkOrderService workorderService, [FromBody] WorkOrderDto workorderDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await workorderService.CreateWorkOrder(workorderDto);
            if (result.WorkOrderID != null && result.WorkOrderID != Guid.Empty)
                return CreatedAtRoute("GetWorkOrderByID", new { id = result.WorkOrderID }, result);
            return BadRequest(new { message = "Failed to create workorder. A workorder with the same name may already exist." });
        }

        // PUT api/<WorkOrderController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateWorkOrder")]
        public async Task<IActionResult> UpdateWorkOrder([FromServices] IWorkOrderService workorderService, Guid id, [FromBody] WorkOrderDto workorderDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await workorderService.UpdateWorkOrder(id, workorderDto);
            if (result == null)
                return NotFound(new { message = "WorkOrder with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<WorkOrderController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteWorkOrder")]
        public async Task<IActionResult> DeleteWorkOrder([FromServices] IWorkOrderService workorderService, Guid id)
        {
            try
            {
                await workorderService.DeleteWorkOrder(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "WorkOrder with the specified ID was not found." });
            }
        }

        // POST: api/workorder/{id}/{status}
        [Authorize(Roles = "User, LandLord, Admin")]
        [HttpPost("{id}/{status}", Name = "UpdateWorkOrderStatus")]
        public async Task<ActionResult> UpdateWorkOrderStatus([FromServices] IWorkOrderService workorderService, [FromRoute] Guid id, [FromRoute] string status)
        {
            var workorder = await workorderService.UpdateWorkOrderStatus(id, status);
            if (workorder == null)
                return NotFound($"WorkOrder with ID {id} not found.");
            return Ok(workorder);
        }
    }
}