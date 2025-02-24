using Microsoft.AspNetCore.Mvc;
using SGS.OAD.AdAuth;

namespace SGS.OAD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AdValidController(ILogger<AdValidController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<bool>> IsValidAsync(string username, string password, string domain = "APAC")
    {
        try
        {
            bool result = await AdAuthHelper.IsValidAsync(username, password, domain);

            if (!result)
            {
                logger.LogWarning("Authorization failed.");
                return Unauthorized("Authorization failed.");
            }

            logger.LogInformation("Authorization succeeded. User: {username}", username);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing your request.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
