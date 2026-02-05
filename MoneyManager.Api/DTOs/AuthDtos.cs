namespace MoneyManager.Api.DTOs;

public record RegisterRequest(string Email, string Password);
public record VerifyOtpRequest(string Email, string OtpCode);
public record LoginRequest(string Email, string Password);
public record AuthTokenResponse(string AccessToken, DateTime ExpiresAtUtc);
