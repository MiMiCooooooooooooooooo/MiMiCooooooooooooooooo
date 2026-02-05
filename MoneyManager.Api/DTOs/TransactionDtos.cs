using MoneyManager.Api.Models;

namespace MoneyManager.Api.DTOs;

public record CreateTransactionRequest(Guid CategoryId, decimal Amount, TransactionType Type, string? Note);
