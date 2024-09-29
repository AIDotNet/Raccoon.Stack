using Raccoon.Stack.Authentication.Identity.Entities.Isolation;

namespace Raccoon.Stack.Authentication.Identity.Core;

public class DefaultMultiTenantUserContext : BaseUserContext, IMultiTenantUserContext
{
    public string? TenantId => GetUser<MultiTenantIdentityUser>()?.TenantId;

    public DefaultMultiTenantUserContext(IUserContext userContext)
        : base()
    {
    }

    public string? GetTenantId<TTenantId>()
    {
        var tenantId = TenantId;

        return tenantId;
    }
}