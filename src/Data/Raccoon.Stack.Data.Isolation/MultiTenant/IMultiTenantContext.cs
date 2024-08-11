namespace Raccoon.Stack.Data.Isolation.MultiTenant;

public interface IMultiTenantContext
{
    Tenant? CurrentTenant { get; }
}