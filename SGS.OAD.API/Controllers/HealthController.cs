using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SGS.OAD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController(HealthCheckService healthCheckService, ILogger<HealthController> logger) : ControllerBase
{
    [HttpGet]
    [EndpointDescription("檢查 API 健康狀態")]
    public async Task<IActionResult> GetHealth()
    {
        logger.LogInformation("Health check requested");

        var report = await healthCheckService.CheckHealthAsync();

        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration,
                description = e.Value.Description
            })
        };

        if (report.Status == HealthStatus.Healthy)
        {
            logger.LogInformation("Health check completed: {Status}", report.Status);
            return Ok(result);
        }
        else
        {
            logger.LogWarning("Health check completed with issues: {Status}", report.Status);
            return StatusCode(503, result); // 503 Service Unavailable for non-healthy status
        }
    }
}
