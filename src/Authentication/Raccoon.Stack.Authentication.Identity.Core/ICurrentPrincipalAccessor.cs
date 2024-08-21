using System.Security.Claims;

namespace Raccoon.Stack.Authentication.Identity.Core;

public interface ICurrentPrincipalAccessor
{
    ClaimsPrincipal? GetCurrentPrincipal();
}
