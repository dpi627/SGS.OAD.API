using Microsoft.AspNetCore.Mvc;
using SGS.OAD.AdAuth;

namespace SGS.OAD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AdValidController(ILogger<AdValidController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> IsValidAsync(string userName, string password, string domain = "APAC")
    {
        if (string.IsNullOrWhiteSpace(userName)||
            string.IsNullOrWhiteSpace(password)||
            string.IsNullOrWhiteSpace(domain))
        {
            logger.LogWarning("Invalid parameter(s). User: {UserName}", userName);
            return BadRequest("Invalid parameter(s).");
        }

        try
        {
            bool valid = await AdAuthHelper.IsValidAsync(userName, password, domain);

            if (!valid)
            {
                logger.LogWarning("Autentication failed. User: {UserName}", userName);
                return Unauthorized(valid);
            }

            logger.LogInformation("Autentication passed. User: {UserName}", userName);
            return Ok(valid);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Autentication error. User: {UserName}", userName);
            return StatusCode(500, "An error occurred.");
        }
    }
}
