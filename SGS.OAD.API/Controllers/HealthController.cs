using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SGS.OAD.API.Models;

namespace SGS.OAD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController(HealthCheckService healthCheckService, ILogger<HealthController> logger) : ControllerBase
{
    [HttpGet]
    [EndpointDescription("檢查 API 健康狀態")]
    [ProducesResponseType(typeof(HealthModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealth()
    {
        try
        {
            var report = await healthCheckService.CheckHealthAsync();

            var result = new HealthModel
            {
                Status = report.Status.ToString(),
                Checks = report.Entries.Select(e => new Checks
                {
                    Name = e.Key,
                    Status = e.Value.Status.ToString(),
                    Duration = e.Value.Duration.ToString(),
                    Description = e.Value.Description
                }).FirstOrDefault()
            };

            if (report.Status == HealthStatus.Healthy)
            {
                logger.LogInformation("Health check completed: Status: {Status}, Report: {@report}", report.Status, report);
                return Ok(result);
            }
            else
            {
                logger.LogWarning("Health check completed with issues: {Status}, Report: {@report}", report.Status, report);
                return StatusCode(503, result); // 503 for non-healthy status
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred.");
            return StatusCode(500, "An error occurred.");
        }
    }
}
