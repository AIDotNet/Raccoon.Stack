namespace Raccoon.Stack.Data.Isolation.Options;

public class IsolationConfigurationOptions
{
    public string TenantId { get; set; }

    public string Environment { get; set; }

    /// <summary>
    /// Used to control the configuration with the highest score when multiple configurations are satisfied. The default score is 100
    /// </summary>
    public int Score { get; set; } = 100;
}

public class IsolationConfigurationOptions<TComponentConfig>: IsolationConfigurationOptions
{
    public TComponentConfig Data { get; set; }
}
