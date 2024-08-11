namespace Raccoon.Stack.Authentication.Identity;

/// <summary>
/// After the user logs in, the tenant information of the current user can be obtained
/// </summary>
public interface IMultiTenantUserContext : IUserContext
{
    string? TenantId { get; }

    TTenantId? GetTenantId<TTenantId>();
}
