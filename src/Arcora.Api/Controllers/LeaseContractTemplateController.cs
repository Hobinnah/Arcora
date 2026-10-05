using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arcora.Api.Controllers;

[Route("api/[controller]/[Action]")]
[ApiController]
public class LeaseContractTemplateController : ControllerBase
{
    [Authorize(Roles = "Viewer, User, LandLord, Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("{organizationID:guid}")]
    public async Task<IActionResult> GetByOrganizationID([FromServices] ILeaseContractTemplateService service, Guid organizationID, [FromQuery] Paging paging)
    {
        return Ok(await service.GetByOrganizationID(organizationID, paging));
    }

    [Authorize(Roles = "Viewer, User, LandLord, Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{templateID:guid}")]
    public async Task<IActionResult> GetByID([FromServices] ILeaseContractTemplateService service, Guid templateID)
    {
        var result = await service.GetByID(templateID);
        if (result == null)
            return NotFound(new { message = "Template was not found." });

        return Ok(result);
    }

    [Authorize(Roles = "Viewer, User, LandLord, Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("{templateID:guid}")]
    public async Task<IActionResult> GetVersions([FromServices] ILeaseContractTemplateService service, Guid templateID)
    {
        return Ok(await service.GetVersions(templateID));
    }

    [Authorize(Roles = "LandLord, Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<IActionResult> Create([FromServices] ILeaseContractTemplateService service, [FromBody] LeaseContractTemplateUpsertDto request)
    {
        try
        {
            var result = await service.CreateTemplate(request);
            if (result.LeaseContractTemplateID == null || result.LeaseContractTemplateID == Guid.Empty)
                return BadRequest(new { message = "Failed to create template." });

            return CreatedAtAction(nameof(GetByID), new { templateID = result.LeaseContractTemplateID }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "LandLord, Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{templateID:guid}")]
    public async Task<IActionResult> Update([FromServices] ILeaseContractTemplateService service, Guid templateID, [FromBody] LeaseContractTemplateUpsertDto request)
    {
        try
        {
            var result = await service.UpdateTemplate(templateID, request);
            if (result == null)
                return NotFound(new { message = "Template was not found." });

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "LandLord, Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{templateID:guid}")]
    public async Task<IActionResult> SetDefault([FromServices] ILeaseContractTemplateService service, Guid templateID, [FromQuery] string? updatedBy)
    {
        var result = await service.SetDefaultTemplate(templateID, updatedBy);
        if (result == null)
            return NotFound(new { message = "Template was not found." });

        return Ok(result);
    }

    [Authorize(Roles = "Viewer, User, LandLord, Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    public async Task<IActionResult> RenderPreview([FromServices] ILeaseContractTemplateService service, [FromBody] LeaseContractRenderRequestDto request)
    {
        var result = await service.RenderContract(request);
        if (result == null)
            return NotFound(new { message = "Lease was not found for rendering." });

        return Ok(result);
    }

    [Authorize(Roles = "Viewer, User, LandLord, Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetStandardTemplate([FromServices] ILeaseContractTemplateService service)
    {
        return Ok(new { html = await service.GetStandardTemplateHtml() });
    }
}
