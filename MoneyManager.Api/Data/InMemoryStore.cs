using MoneyManager.Api.Models;

namespace MoneyManager.Api.Data;

public class InMemoryStore
{
    public List<User> Users { get; } = [];
    public List<Category> Categories { get; } = [];
    public List<TransactionEntry> Transactions { get; } = [];
    public List<Budget> Budgets { get; } = [];
    public Dictionary<string, string> OtpByEmail { get; } = new(StringComparer.OrdinalIgnoreCase);

    public static readonly string[] DefaultCategoryNames =
    [
        "Ovqat",
        "Transport",
        "Uy",
        "Salomatlik",
        "O'yin-kulgi",
        "Boshqa"
    ];
}
