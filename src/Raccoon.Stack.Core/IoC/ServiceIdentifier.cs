﻿namespace Raccoon.Stack.Core.IoC;

/// <summary>
/// see：
/// https://github.com/dotnet/runtime/blob/release/8.0/src/libraries/Microsoft.Extensions.DependencyInjection/src/ServiceLookup/ServiceIdentifier.cs#L9
/// </summary>
public readonly struct ServiceIdentifier : IEquatable<ServiceIdentifier>
{
    public object ServiceKey { get; }

    public Type ServiceType { get; }

    public ServiceIdentifier(Type serviceType)
    {
        ServiceType = serviceType;
        ServiceKey = null;
    }

    // ReSharper disable once ConvertToPrimaryConstructor
    public ServiceIdentifier(object serviceKey, Type serviceType)
    {
        ServiceKey = serviceKey;
        ServiceType = serviceType;
    }

    public bool Equals(ServiceIdentifier other)
    {
        if (ServiceKey == null && other.ServiceKey == null)
        {
            return ServiceType == other.ServiceType;
        }

        if (ServiceKey != null && other.ServiceKey != null)
        {
            return ServiceType == other.ServiceType && ServiceKey.Equals(other.ServiceKey);
        }

        return false;
    }

    public override bool Equals(object obj)
    {
        return obj is ServiceIdentifier identifier && Equals(identifier);
    }

    public override int GetHashCode()
    {
        if (ServiceKey == null)
        {
            return ServiceType.GetHashCode();
        }

        unchecked
        {
            return (ServiceType.GetHashCode() * 397) ^ ServiceKey.GetHashCode();
        }
    }
}