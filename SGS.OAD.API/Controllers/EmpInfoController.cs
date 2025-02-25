using Microsoft.AspNetCore.Mvc;
using SGS.OAD.HrInfo;

namespace SGS.OAD.API.Controllers;

[Route("[controller]")]
[ApiController]
public class EmpInfoController(ILogger<EmpIdController> logger) : ControllerBase
{
    [HttpGet]
    [EndpointDescription("透過 AD 帳號或工號 (二擇一) 取得員工資訊")]
    [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEmpInfoAsync(string id, [FromQuery] IdType idType = IdType.AdId)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            logger.LogWarning("Invalid Id: {Id}", id);
            return BadRequest("Invalid Id.");
        }

        try
        {
            var helper = await HrInfoHelper.Create().BuildAsync();
            var emp = idType == IdType.AdId ? await helper.GetByAdIdAsync(id) : await helper.GetByEmpIdAsync(id) ;

            if (emp == default || emp.StfCode == default)
            {
                logger.LogWarning("Can't found EmpInfo. Id: {Id}", id);
                return NotFound($"Can't found EmpInfo. Id: {id}");
            }

            logger.LogInformation("Get EmpInfo: {EmpInfo}", emp);
            return Ok(emp);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred. Id: {Id}", id);
            return StatusCode(500, "An error occurred.");
        }
    }

    public enum IdType
    {
        AdId,
        EmpId
    }
}
