using Microsoft.AspNetCore.Mvc;
using SGS.OAD.HrInfo;

namespace SGS.OAD.API.Controllers;

[Route("[controller]")]
[ApiController]
public class EmpController(ILogger<EmpController> logger) : ControllerBase
{
    [HttpGet]
    [EndpointDescription("透過 AD 帳號取得工號")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEmpIdAsync(string adAccount)
    {
        if (string.IsNullOrWhiteSpace(adAccount))
        {
            logger.LogWarning("Invalid AdAccount: {AdAccount}", adAccount);
            return BadRequest("Invalid AdAccount.");
        }

        try
        {
            var hrInfo = await HrInfoHelper.Create().BuildAsync();
            var empId = await hrInfo.GetEmpIdAsync(adAccount);

            if (empId == default)
            {
                logger.LogWarning("Can't found EmpId. AdAccount: {AdAccount}", adAccount);
                return NotFound($"Can't found EmpId. AdAccount: {adAccount}");
            }

            logger.LogInformation("Get EmpId: {EmpId}", empId);
            return Ok(empId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred. AdAccount: {AdAccount}", adAccount);
            return StatusCode(500, "An error occurred.");
        }
    }

    /// <summary>
    /// 透過 AD 帳號或工號 (二擇一) 取得員工資訊
    /// </summary>
    /// <param name="id">AD 帳號或工號</param>
    /// <param name="idType">識別類型 (AD帳號 或工號)</param>
    /// <returns>員工資訊</returns>
    /// <response code="200">成功取得員工資訊</response>
    /// <response code="400">無效的 ID</response>
    /// <response code="404">找不到員工資訊</response>
    /// <response code="500">伺服器錯誤</response>
    [HttpPost]
    [EndpointDescription("透過 AD 帳號或工號 (二擇一) 取得員工資訊")]
    [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEmpInfoAsync(string id, IdType idType = IdType.Ad)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            logger.LogWarning("Invalid Id: {Id}", id);
            return BadRequest("Invalid Id.");
        }

        try
        {
            var hrInfo = await HrInfoHelper.Create().BuildAsync();
            var emp = idType == IdType.Ad ? await hrInfo.GetByAdIdAsync(id) : await hrInfo.GetByEmpIdAsync(id);

            if (emp == default || emp.StfCode == default)
            {
                logger.LogWarning("Can't found EmpInfo. Id: {Id}", id);
                return NotFound($"Can't found EmpInfo. Id: {id}");
            }

            logger.LogInformation("Get EmpInfo: {@EmpInfo}", emp);
            return Ok(emp);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred. Id: {Id}", id);
            return StatusCode(500, "An error occurred.");
        }
    }

    /// <summary>
    /// ID類型
    /// </summary>
    public enum IdType
    {
        /// <summary>
        /// AD 帳號
        /// </summary>
        Ad,
        /// <summary>
        /// 工號
        /// </summary>
        Emp
    }
}
