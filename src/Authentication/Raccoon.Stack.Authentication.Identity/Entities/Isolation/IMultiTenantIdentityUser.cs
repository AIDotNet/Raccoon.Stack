namespace Raccoon.Stack.Authentication.Identity.Entities.Isolation;

public interface IMultiTenantIdentityUser : IIdentityUser
{
    string? TenantId { get; set; }
}
