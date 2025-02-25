using Microsoft.AspNetCore.Mvc;
using SGS.OAD.AdAuth;
using SGS.OAD.API.Models;

namespace SGS.OAD.API.Controllers;

[Route("[controller]")]
[ApiController]
public class AdController(ILogger<AdController> logger) : ControllerBase
{
    [HttpGet]
    [EndpointDescription("透過 AD 帳號與密碼進行驗證")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> IsValidAsync(string userName, string password, string domain = "APAC")
    {
        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password) ||
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

    [HttpPost]
    [EndpointDescription("透過 AD 帳號與密碼進行驗證，通過後取得部分 AD 資訊")]
    [ProducesResponseType(typeof(AdInfoModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAdInfoAsync(string userName, string password, string domain = "APAC")
    {
        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(domain))
        {
            logger.LogWarning("Invalid parameter(s). User: {UserName}", userName);
            return BadRequest("Invalid parameter(s).");
        }

        try
        {
            var adInfo = await AdAuthHelper.GetInfoAsync(userName, password, domain);

            if (adInfo == default)
            {
                logger.LogWarning("Can't find AdInfo. User: {UserName}", userName);
                return Unauthorized($"Can't find AdInfo. User: {userName}");
            }

            logger.LogInformation("Found AdInfo: {@AdInfo}", adInfo);
            return Ok(adInfo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred. User: {UserName}", userName);
            return StatusCode(500, "Error occurred.");
        }
    }
}
