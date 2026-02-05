using Microsoft.AspNetCore.Mvc;
using MoneyManager.Api.DTOs;
using MoneyManager.Api.Services;

namespace MoneyManager.Api.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionsController : BaseAuthorizedController
{
    private readonly FinanceService _finance;

    public TransactionsController(FinanceService finance)
    {
        _finance = finance;
    }

    [HttpPost]
    public IActionResult Create(CreateTransactionRequest request)
    {
        var (entry, message, budgetExceeded) = _finance.AddTransaction(
            CurrentUserId,
            request.CategoryId,
            request.Amount,
            request.Type,
            request.Note);

        if (entry is null)
        {
            return BadRequest(new { message });
        }

        return Ok(new
        {
            message,
            budgetExceeded,
            entry
        });
    }
}
