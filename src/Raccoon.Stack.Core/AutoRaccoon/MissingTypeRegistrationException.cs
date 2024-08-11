namespace Raccoon.Stack.Core.AutoRaccoon;

public class MissingTypeRegistrationException : InvalidOperationException
{
    public MissingTypeRegistrationException(Type serviceType)
        : base($"Could not find any registered services for type '{serviceType.ToFriendlyName()}'.")
    {
        ServiceType = serviceType;
    }

    public Type ServiceType { get; }
}