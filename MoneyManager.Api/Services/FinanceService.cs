using ClosedXML.Excel;
using MoneyManager.Api.Data;
using MoneyManager.Api.Models;

namespace MoneyManager.Api.Services;

public class FinanceService
{
    private readonly InMemoryStore _store;

    public FinanceService(InMemoryStore store)
    {
        _store = store;
    }

    public IReadOnlyCollection<Category> GetCategories(Guid userId)
        => _store.Categories.Where(c => c.UserId == userId).OrderBy(c => c.Name).ToList();

    public Category AddCategory(Guid userId, string name)
    {
        var category = new Category
        {
            UserId = userId,
            Name = name,
            IsDefault = false
        };

        _store.Categories.Add(category);
        return category;
    }

    public (TransactionEntry? Entry, string Message, bool BudgetExceeded) AddTransaction(
        Guid userId,
        Guid categoryId,
        decimal amount,
        TransactionType type,
        string? note)
    {
        var category = _store.Categories.FirstOrDefault(c => c.Id == categoryId && c.UserId == userId);
        if (category is null)
        {
            return (null, "Category not found.", false);
        }

        var entry = new TransactionEntry
        {
            UserId = userId,
            CategoryId = categoryId,
            Amount = amount,
            Type = type,
            Note = note,
            CreatedAtUtc = DateTime.UtcNow
        };

        _store.Transactions.Add(entry);

        var budgetExceeded = false;
        if (type == TransactionType.Expense)
        {
            var month = entry.CreatedAtUtc.Month;
            var year = entry.CreatedAtUtc.Year;
            var budget = _store.Budgets.FirstOrDefault(b => b.UserId == userId && b.CategoryId == categoryId && b.Year == year && b.Month == month);
            if (budget is not null)
            {
                var spent = _store.Transactions
                    .Where(t => t.UserId == userId
                                && t.CategoryId == categoryId
                                && t.Type == TransactionType.Expense
                                && t.CreatedAtUtc.Year == year
                                && t.CreatedAtUtc.Month == month)
                    .Sum(t => t.Amount);

                budgetExceeded = spent > budget.LimitAmount;
            }
        }

        return (entry, "Transaction saved.", budgetExceeded);
    }

    public Budget SetBudget(Guid userId, Guid categoryId, int year, int month, decimal limitAmount)
    {
        var existing = _store.Budgets.FirstOrDefault(b =>
            b.UserId == userId && b.CategoryId == categoryId && b.Year == year && b.Month == month);

        if (existing is null)
        {
            existing = new Budget
            {
                UserId = userId,
                CategoryId = categoryId,
                Year = year,
                Month = month,
                LimitAmount = limitAmount
            };
            _store.Budgets.Add(existing);
        }
        else
        {
            existing.LimitAmount = limitAmount;
        }

        return existing;
    }

    public object BuildReport(Guid userId, DateTime fromUtc, DateTime toUtc)
    {
        var tx = _store.Transactions
            .Where(t => t.UserId == userId && t.CreatedAtUtc >= fromUtc && t.CreatedAtUtc <= toUtc)
            .ToList();

        var income = tx.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var expense = tx.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        var byCategory = tx
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => t.CategoryId)
            .Select(group =>
            {
                var categoryName = _store.Categories.FirstOrDefault(c => c.Id == group.Key)?.Name ?? "Unknown";
                return new
                {
                    CategoryId = group.Key,
                    CategoryName = categoryName,
                    TotalExpense = group.Sum(x => x.Amount)
                };
            })
            .OrderByDescending(x => x.TotalExpense)
            .ToList();

        return new
        {
            FromUtc = fromUtc,
            ToUtc = toUtc,
            Income = income,
            Expense = expense,
            Balance = income - expense,
            SavingsRatePercent = income == 0 ? 0 : ((income - expense) / income) * 100,
            CategoryBreakdown = byCategory
        };
    }

    public byte[] ExportToExcel(Guid userId, DateTime fromUtc, DateTime toUtc)
    {
        var tx = _store.Transactions
            .Where(t => t.UserId == userId && t.CreatedAtUtc >= fromUtc && t.CreatedAtUtc <= toUtc)
            .OrderBy(t => t.CreatedAtUtc)
            .ToList();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Transactions");

        ws.Cell(1, 1).Value = "Date";
        ws.Cell(1, 2).Value = "Type";
        ws.Cell(1, 3).Value = "Category";
        ws.Cell(1, 4).Value = "Amount";
        ws.Cell(1, 5).Value = "Note";

        for (var i = 0; i < tx.Count; i++)
        {
            var row = i + 2;
            var category = _store.Categories.FirstOrDefault(c => c.Id == tx[i].CategoryId)?.Name ?? "Unknown";

            ws.Cell(row, 1).Value = tx[i].CreatedAtUtc;
            ws.Cell(row, 2).Value = tx[i].Type.ToString();
            ws.Cell(row, 3).Value = category;
            ws.Cell(row, 4).Value = tx[i].Amount;
            ws.Cell(row, 5).Value = tx[i].Note ?? string.Empty;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
