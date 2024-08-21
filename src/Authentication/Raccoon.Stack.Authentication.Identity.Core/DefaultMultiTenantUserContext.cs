using Raccoon.Stack.Authentication.Identity.Entities.Isolation;
using Raccoon.Stack.EntityFrameworkCore;

namespace Raccoon.Stack.Authentication.Identity.Core;

public class DefaultMultiTenantUserContext : BaseUserContext, IMultiTenantUserContext
{
    public string? TenantId => GetUser<MultiTenantIdentityUser>()?.TenantId;

    public DefaultMultiTenantUserContext(IUserContext userContext, ITypeConvertProvider typeConvertProvider)
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
