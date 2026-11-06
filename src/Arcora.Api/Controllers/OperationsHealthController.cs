using Arcora.Api.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arcora.Api.Controllers;

[ApiController]
[Route("api/operations/health")]
[Authorize(Roles = "Admin")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class OperationsHealthController(OperationsRegistry registry) : ControllerBase
{
    [HttpGet]
    public OperationsSnapshot Get() => registry.Snapshot();
}

[ApiController]
[AllowAnonymous]
[Route("health")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class HealthProbeController(OperationsRegistry registry) : ControllerBase
{
    [HttpGet("live")]
    public IActionResult Live() => Ok(new { status = "Alive" });

    [HttpGet("ready")]
    public IActionResult Ready()
    {
        var checks = registry.Snapshot().Checks;
        var check = checks.SingleOrDefault(x => x.Id == "dependency.database");
        var ready = check?.Status == "Healthy" &&
            !checks.Any(x => x.Id == "dependency.key-vault" && x.Status == "Failed");
        return StatusCode(ready ? 200 : 503, new { status = ready ? "Ready" : "NotReady" });
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        var snapshot = registry.Snapshot();
        var monitor = snapshot.Checks.SingleOrDefault(x => x.Id == "monitor.cycle");
        var healthy = monitor?.Status == "Healthy" && snapshot.OverallStatus == "Healthy";
        return StatusCode(healthy ? 200 : 503, new { status = healthy ? "Healthy" : "AttentionRequired" });
    }
}
