using Microsoft.AspNetCore.Mvc;
using MoneyManager.Api.DTOs;
using MoneyManager.Api.Services;

namespace MoneyManager.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        var result = _authService.Register(request.Email, request.Password);
        if (!result.Success)
        {
            return BadRequest(new { result.Message });
        }

        return Ok(new { result.Message });
    }

    [HttpPost("verify-otp")]
    public IActionResult VerifyOtp(VerifyOtpRequest request)
    {
        var result = _authService.VerifyOtp(request.Email, request.OtpCode);
        if (!result.Success)
        {
            return BadRequest(new { result.Message });
        }

        return Ok(new { result.Message });
    }

    [HttpPost("login")]
    public ActionResult<AuthTokenResponse> Login(LoginRequest request)
    {
        var result = _authService.Login(request.Email, request.Password);
        if (!result.Success)
        {
            return Unauthorized(new { result.Message });
        }

        return Ok(new AuthTokenResponse(result.Token, result.ExpiresAtUtc));
    }
}
