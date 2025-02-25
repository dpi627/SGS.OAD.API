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
    public async Task<IActionResult> IsValidAsync(string UserName, string Password, string Domain = "APAC")
    {
        if (string.IsNullOrWhiteSpace(UserName)||
            string.IsNullOrWhiteSpace(Password)||
            string.IsNullOrWhiteSpace(Domain))
        {
            logger.LogWarning("Invalid parameter(s). User: {UserName}", UserName);
            return BadRequest("Invalid parameter(s).");
        }

        try
        {
            bool valid = await AdAuthHelper.IsValidAsync(UserName, Password, Domain);

            if (!valid)
            {
                logger.LogWarning("Autentication failed. User: {UserName}", UserName);
                return Unauthorized(valid);
            }

            logger.LogInformation("Autentication passed. User: {UserName}", UserName);
            return Ok(valid);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Autentication error. User: {UserName}", UserName);
            return StatusCode(500, "An error occurred.");
        }
    }
}
