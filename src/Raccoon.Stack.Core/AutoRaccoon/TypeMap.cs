namespace Raccoon.Stack.Core.AutoRaccoon;

internal struct TypeMap
{
    public TypeMap(Type implementationType, IEnumerable<Type> serviceTypes)
    {
        ImplementationType = implementationType;
        ServiceTypes = serviceTypes;
    }

    public Type ImplementationType { get; }

    public IEnumerable<Type> ServiceTypes { get; }
}