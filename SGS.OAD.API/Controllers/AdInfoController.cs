using Microsoft.AspNetCore.Mvc;
using SGS.OAD.AdAuth;
using SGS.OAD.API.Models;

namespace SGS.OAD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AdInfoController(ILogger<AdValidController> logger) : ControllerBase
{
    [HttpPost]
    [EndpointDescription("透過 AD 帳號與密碼，取得部分 AD 資訊")]
    [ProducesResponseType(typeof(AdInfoModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAdInfoAsync(AdValidModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Password = string.Empty;
            logger.LogWarning("Invalid model state. Model: {model}", model);
            return BadRequest("Invalid model state.");
        }

        try
        {
            var helper = await AdAuthHelper.GetInfoAsync(model.UserName, model.Password, model.Domain);

            if (helper == default)
            {
                logger.LogWarning("Can't find AdInfo. User: {UserName}", model.UserName);
                return Unauthorized($"Can't find AdInfo. User: {model.UserName}");
            }

            logger.LogInformation("Found AdInfo: {@AdInfo}", helper);
            return Ok(helper);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred. User: {username}", model.UserName);
            return StatusCode(500, "Error occurred.");
        }
    }
}
