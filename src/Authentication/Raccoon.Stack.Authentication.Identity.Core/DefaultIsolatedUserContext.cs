using Raccoon.Stack.Authentication.Identity.Entities.Isolation;
using Raccoon.Stack.EntityFrameworkCore;

namespace Raccoon.Stack.Authentication.Identity.Core;

public class DefaultIsolatedUserContext : BaseUserContext, IIsolatedUserContext
{
    public string? TenantId => GetUser<IsolatedIdentityUser>()?.TenantId;

    public string? Environment => GetUser<IsolatedIdentityUser>()?.Environment;

    public DefaultIsolatedUserContext(IUserContext userContext, ITypeConvertProvider typeConvertProvider)
        : base(userContext, typeConvertProvider)
    {
    }

    public TTenantId? GetTenantId<TTenantId>()
    {
        var tenantId = TenantId;
        if (tenantId == null)
            return default;

        return TypeConvertProvider.ConvertTo<TTenantId>(tenantId);
    }
}
