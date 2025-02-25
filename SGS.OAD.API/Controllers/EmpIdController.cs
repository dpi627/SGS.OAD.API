using Microsoft.AspNetCore.Mvc;
using SGS.OAD.HrInfo;

namespace SGS.OAD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class EmpIdController(ILogger<EmpIdController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEmpIdAsync(string AdAccount)
    {
        if (string.IsNullOrWhiteSpace(AdAccount))
        {
            logger.LogWarning("Invalid AdAccount: {AdAccount}", AdAccount);
            return BadRequest("Invalid AdAccount.");
        }

        try
        {
            var helper = await HrInfoHelper.Create().BuildAsync();
            var empId = await helper.GetEmpIdAsync(AdAccount);

            if (empId == default)
            {
                logger.LogWarning("Can't found EmpId. AdAccount: {AdAccount}", AdAccount);
                return NotFound($"Can't found EmpId. AdAccount: {AdAccount}");
            }

            logger.LogInformation("Get EmpId: {EmpId}", helper);
            return Ok(empId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred. AdAccount: {AdAccount}", AdAccount);
            return StatusCode(500, "An error occurred.");
        }
    }
}
