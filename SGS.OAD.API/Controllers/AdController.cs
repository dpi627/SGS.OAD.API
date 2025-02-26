using Microsoft.AspNetCore.Mvc;
using SGS.OAD.AdAuth;
using SGS.OAD.API.Models;

namespace SGS.OAD.API.Controllers;

[Route("[controller]")]
[ApiController]
public class AdController(ILogger<AdController> logger) : ControllerBase
{
    [HttpPost("valid")]
    [EndpointDescription("驗證 AD 帳號密碼，取得驗證結果")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> IsValidAsync(AdRequest model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName) ||
            string.IsNullOrWhiteSpace(model.Password) ||
            string.IsNullOrWhiteSpace(model.Domain))
        {
            logger.LogWarning("Invalid parameter(s). User: {AdAccount}", model.UserName);
            return BadRequest("Invalid parameter(s).");
        }

        try
        {
            bool valid = await AdAuthHelper.IsValidAsync(model.UserName, model.Password, model.Domain);

            if (!valid)
            {
                logger.LogWarning("Autentication failed. User: {AdAccount}", model.UserName);
                return Unauthorized(valid);
            }

            logger.LogInformation("Autentication passed. User: {AdAccount}", model.UserName);
            return Ok(valid);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Autentication error. User: {AdAccount}", model.UserName);
            return StatusCode(500, "An error occurred.");
        }
    }

    [HttpPost("info")]
    [EndpointDescription("驗證 AD 帳號與密碼，取得 AD 開放資訊(無個資)")]
    [ProducesResponseType(typeof(AdInfoModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAdInfoAsync(AdRequest model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName) ||
            string.IsNullOrWhiteSpace(model.Password) ||
            string.IsNullOrWhiteSpace(model.Domain))
        {
            logger.LogWarning("Invalid parameter(s). User: {AdAccount}", model.UserName);
            return BadRequest("Invalid parameter(s).");
        }

        try
        {
            var adInfo = await AdAuthHelper.GetInfoAsync(model.UserName, model.Password, model.Domain);

            if (adInfo == default)
            {
                logger.LogWarning("Can't find AdInfo. User: {AdAccount}", model.UserName);
                return Unauthorized($"Can't find AdInfo. User: {model.UserName}");
            }

            logger.LogInformation("Found AdInfo: {@AdInfo}", adInfo);
            return Ok(adInfo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred. User: {AdAccount}", model.UserName);
            return StatusCode(500, "Error occurred.");
        }
    }
}
