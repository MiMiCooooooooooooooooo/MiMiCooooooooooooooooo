using Microsoft.AspNetCore.Mvc;
using MoneyManager.Api.Services;

namespace MoneyManager.Api.Controllers;

[ApiController]
[Route("api/export")]
public class ExportController : BaseAuthorizedController
{
    private readonly FinanceService _finance;

    public ExportController(FinanceService finance)
    {
        _finance = finance;
    }

    [HttpGet("xlsx")]
    public IActionResult ExportXlsx([FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc)
    {
        var to = toUtc ?? DateTime.UtcNow;
        var from = fromUtc ?? to.AddMonths(-1);

        var bytes = _finance.ExportToExcel(CurrentUserId, from, to);
        var fileName = $"transactions_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx";

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
