namespace MoneyManager.Api.Models;

public enum TransactionType
{
    Income,
    Expense
}

public class TransactionEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public TransactionType Type { get; set; }
}
