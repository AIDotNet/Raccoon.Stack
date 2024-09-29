using Raccoon.Stack.Authentication.Identity.Entities.Isolation;

namespace Raccoon.Stack.Authentication.Identity.Core;

public class DefaultIsolatedUserContext : BaseUserContext, IIsolatedUserContext
{
    public string? TenantId => GetUser<IsolatedIdentityUser>()?.TenantId;

    public string? Environment => GetUser<IsolatedIdentityUser>()?.Environment;

    public DefaultIsolatedUserContext(IUserContext userContext)
        : base()
    {
    }

    public string? GetTenantId<TTenantId>()
    {
        var tenantId = TenantId;


        return tenantId;
    }
}