namespace Raccoon.Stack.Authentication.Identity;

internal sealed class DisposeAction : IDisposable
{
    private readonly Action _action;

    public DisposeAction(Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        _action = action;
    }

    public void Dispose() => _action();
}
