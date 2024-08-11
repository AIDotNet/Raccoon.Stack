namespace Raccoon.Stack.Data.Isolation.MultiEnvironment;

public interface IMultiEnvironment
{
    /// <summary>
    /// The framework is responsible for the assignment operation, no manual assignment is required
    /// </summary>
    public string Environment { get; set; }
}
