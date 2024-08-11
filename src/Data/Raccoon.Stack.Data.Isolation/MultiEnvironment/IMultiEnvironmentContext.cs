namespace Raccoon.Stack.Data.Isolation.MultiEnvironment;

public interface IMultiEnvironmentContext
{
    string CurrentEnvironment { get; }
}