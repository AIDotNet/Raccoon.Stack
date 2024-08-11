using Raccoon.Stack.Data.Contracts.Options;

namespace Raccoon.Stack.Data.Options;

public class DeserializerFactoryOptions : RaccoonFactoryOptions<RaccoonRelationOptions<IDeserializer>>
{
    public void TryAdd(string name, Func<IServiceProvider, IDeserializer> func)
    {
        if (Options.Any(opt => opt.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException(
                $"The deserializer name already exists, please change the name, the repeat name is [{name}]");

        Options.Add(new RaccoonRelationOptions<IDeserializer>(name, func));
    }
}
