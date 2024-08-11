namespace Raccoon.Stack.Data.Isolation.Options;

public class IsolationOptions
{
    internal string? SectionName { get; set; }

    public Type MultiTenantIdType { get; set; }

    public bool Enable => EnableMultiTenant || EnableMultiEnvironment;

    private bool _enableMultiTenant;

    private bool _enableMultiEnvironment;
    internal bool EnableMultiTenant => _enableMultiTenant;
    internal bool EnableMultiEnvironment => _enableMultiEnvironment;

    private string _multiTenantIdName;

    internal string MultiTenantIdName
    {
        get => _multiTenantIdName;
        set
        {
            _enableMultiTenant = true;
            _multiTenantIdName = value;
        }
    }

    private string _multiEnvironmentName;

    internal string MultiEnvironmentName
    {
        get => _multiEnvironmentName;
        set
        {
            _enableMultiEnvironment = true;
            _multiEnvironmentName = value;
        }
    }

    public IsolationOptions()
    {
        MultiTenantIdType = typeof(Guid);
    }
}