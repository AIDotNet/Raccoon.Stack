using Raccoon.Stack.Authentication.Identity.Entities.Isolation;

namespace Raccoon.Stack.Authentication.Identity.Core;

public class DefaultMultiEnvironmentUserContext : BaseUserContext, IMultiEnvironmentUserContext
{
    public string? Environment => GetUser<MultiEnvironmentIdentityUser>()?.Environment;

    public DefaultMultiEnvironmentUserContext(IUserContext userContext)
        : base()
    {
    }
}
