using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MoneyManager.Api.Data;
using MoneyManager.Api.Models;

namespace MoneyManager.Api.Services;

public class AuthService
{
    private readonly InMemoryStore _store;
    private readonly IConfiguration _configuration;

    public AuthService(InMemoryStore store, IConfiguration configuration)
    {
        _store = store;
        _configuration = configuration;
    }

    public (bool Success, string Message) Register(string email, string password)
    {
        if (_store.Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            return (false, "User already exists.");
        }

        _store.Users.Add(new User
        {
            Email = email,
            PasswordHash = Hash(password),
            IsVerified = false
        });

        var otp = Random.Shared.Next(100000, 999999).ToString();
        _store.OtpByEmail[email] = otp;

        return (true, $"Registered. OTP sent (demo): {otp}");
    }

    public (bool Success, string Message) VerifyOtp(string email, string otpCode)
    {
        var user = _store.Users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (user is null)
        {
            return (false, "User not found.");
        }

        if (!_store.OtpByEmail.TryGetValue(email, out var storedOtp) || storedOtp != otpCode)
        {
            return (false, "Invalid OTP.");
        }

        user.IsVerified = true;
        _store.OtpByEmail.Remove(email);
        SeedDefaultCategories(user.Id);

        return (true, "User verified.");
    }

    public (bool Success, string Message, string Token, DateTime ExpiresAtUtc) Login(string email, string password)
    {
        var user = _store.Users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (user is null || user.PasswordHash != Hash(password))
        {
            return (false, "Invalid credentials.", string.Empty, DateTime.MinValue);
        }

        if (!user.IsVerified)
        {
            return (false, "Account is not verified.", string.Empty, DateTime.MinValue);
        }

        var token = CreateToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(8);
        return (true, "Success", token, expiresAt);
    }

    private string CreateToken(User user)
    {
        var jwt = _configuration.GetSection("Jwt");
        var key = jwt["Key"] ?? throw new InvalidOperationException("JWT Key missing");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string Hash(string text)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        return Convert.ToHexString(bytes);
    }

    private void SeedDefaultCategories(Guid userId)
    {
        if (_store.Categories.Any(c => c.UserId == userId && c.IsDefault))
        {
            return;
        }

        foreach (var name in InMemoryStore.DefaultCategoryNames)
        {
            _store.Categories.Add(new Category
            {
                UserId = userId,
                Name = name,
                IsDefault = true
            });
        }
    }
}
