using Microsoft.AspNetCore.Mvc;
using MoneyManager.Api.Services;

namespace MoneyManager.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : BaseAuthorizedController
{
    private readonly FinanceService _finance;

    public ReportsController(FinanceService finance)
    {
        _finance = finance;
    }

    [HttpGet("weekly")]
    public IActionResult Weekly()
    {
        var to = DateTime.UtcNow;
        var from = to.AddDays(-7);
        return Ok(_finance.BuildReport(CurrentUserId, from, to));
    }

    [HttpGet("monthly")]
    public IActionResult Monthly()
    {
        var to = DateTime.UtcNow;
        var from = to.AddMonths(-1);
        return Ok(_finance.BuildReport(CurrentUserId, from, to));
    }

    [HttpGet("yearly")]
    public IActionResult Yearly()
    {
        var to = DateTime.UtcNow;
        var from = to.AddYears(-1);
        return Ok(_finance.BuildReport(CurrentUserId, from, to));
    }
}
