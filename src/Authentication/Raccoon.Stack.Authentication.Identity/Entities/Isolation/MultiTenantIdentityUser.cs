namespace Raccoon.Stack.Authentication.Identity.Entities.Isolation;

public class MultiTenantIdentityUser : IdentityUser, IMultiTenantIdentityUser
{
    public string? TenantId { get; set; }
}
