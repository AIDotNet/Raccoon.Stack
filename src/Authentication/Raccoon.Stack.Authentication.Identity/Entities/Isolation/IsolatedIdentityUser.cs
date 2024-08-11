namespace Raccoon.Stack.Authentication.Identity.Entities.Isolation;

public class IsolatedIdentityUser : MultiTenantIdentityUser
{
    public string? Environment { get; set; }
}
