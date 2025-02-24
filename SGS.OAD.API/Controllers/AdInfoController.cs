using Microsoft.AspNetCore.Mvc;
using SGS.OAD.AdAuth;
using SGS.OAD.API.Models;

namespace SGS.OAD.API.Controllers;

[Route("[controller]")]
[ApiController]
public class AdInfoController(ILogger<AdValidController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AdInfoModel>> GetAdInfoAsync(AdValidModel model)
    {
        try
        {
            var result = await AdAuthHelper.GetInfoAsync(model.UserName, model.Password, model.Domain);

            if (result == default)
            {
                logger.LogWarning("Authorization failed.");
                return Unauthorized("Authorization failed.");
            }

            logger.LogInformation("Get User: {@UserInfo}", result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing your request.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
