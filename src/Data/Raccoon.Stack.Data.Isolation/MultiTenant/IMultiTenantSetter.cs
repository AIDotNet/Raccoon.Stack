namespace Raccoon.Stack.Data.Isolation.MultiTenant;

public interface IMultiTenantSetter
{
    void SetTenant(Tenant? tenant);
}
