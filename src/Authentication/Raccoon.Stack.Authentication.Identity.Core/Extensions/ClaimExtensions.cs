using System.Security.Claims;

namespace Raccoon.Stack.Authentication.Identity.Core.Extensions;


public static class ClaimExtensions
{
    public static string? FindClaimValue(this ClaimsPrincipal claimsPrincipal, string claimType)
        => claimsPrincipal.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
}
