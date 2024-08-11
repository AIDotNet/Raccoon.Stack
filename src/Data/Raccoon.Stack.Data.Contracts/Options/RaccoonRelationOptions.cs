namespace Raccoon.Stack.Data.Contracts.Options;

public class RaccoonRelationOptions
{
    public string Name { get; protected set; }
}

public class RaccoonRelationOptions<TService> : RaccoonRelationOptions
    where TService : class
{
    public Func<IServiceProvider, TService> Func { get; set; }

    public RaccoonRelationOptions(string name) => Name = name;

    public RaccoonRelationOptions(string name, Func<IServiceProvider, TService> func) : this(name)
    {
        Func = func;
    }
}
