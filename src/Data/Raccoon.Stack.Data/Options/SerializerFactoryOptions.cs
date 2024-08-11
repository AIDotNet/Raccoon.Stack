using Raccoon.Stack.Data.Contracts.Options;

namespace Raccoon.Stack.Data.Options;


public class SerializerFactoryOptions : RaccoonFactoryOptions<RaccoonRelationOptions<ISerializer>>
{
    public void TryAdd(string name, Func<IServiceProvider, ISerializer> func)
    {
        if (Options.Any(opt => opt.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException(
                $"The serializer name already exists, please change the name, the repeat name is [{name}]");

        Options.Add(new RaccoonRelationOptions<ISerializer>(name, func));
    }
}
