namespace MoneyManager.Api.DTOs;

public record SetBudgetRequest(Guid CategoryId, int Year, int Month, decimal LimitAmount);
