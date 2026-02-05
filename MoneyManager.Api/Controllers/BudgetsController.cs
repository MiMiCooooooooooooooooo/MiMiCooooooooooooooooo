using Microsoft.AspNetCore.Mvc;
using MoneyManager.Api.DTOs;
using MoneyManager.Api.Services;

namespace MoneyManager.Api.Controllers;

[ApiController]
[Route("api/budgets")]
public class BudgetsController : BaseAuthorizedController
{
    private readonly FinanceService _finance;

    public BudgetsController(FinanceService finance)
    {
        _finance = finance;
    }

    [HttpPost]
    public IActionResult SetBudget(SetBudgetRequest request)
    {
        var budget = _finance.SetBudget(CurrentUserId, request.CategoryId, request.Year, request.Month, request.LimitAmount);
        return Ok(budget);
    }
}
