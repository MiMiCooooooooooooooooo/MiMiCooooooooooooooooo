using Microsoft.AspNetCore.Mvc;
using MoneyManager.Api.DTOs;
using MoneyManager.Api.Services;

namespace MoneyManager.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : BaseAuthorizedController
{
    private readonly FinanceService _finance;

    public CategoriesController(FinanceService finance)
    {
        _finance = finance;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_finance.GetCategories(CurrentUserId));
    }

    [HttpPost]
    public IActionResult Create(CreateCategoryRequest request)
    {
        var category = _finance.AddCategory(CurrentUserId, request.Name);
        return Ok(category);
    }
}
