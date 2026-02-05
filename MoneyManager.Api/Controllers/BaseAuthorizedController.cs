using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MoneyManager.Api.Controllers;

[Authorize]
public abstract class BaseAuthorizedController : ControllerBase
{
    protected Guid CurrentUserId
    {
        get
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(sub, out var userId)
                ? userId
                : throw new UnauthorizedAccessException("Invalid token subject.");
        }
    }
}
