namespace Raccoon.Stack.Data.Isolation.Options;

public class IsolationOptions<TComponentConfig>
{
    public List<IsolationConfigurationOptions<TComponentConfig>> Data { get; set; } = new();
}
