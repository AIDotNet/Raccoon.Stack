namespace Raccoon.Stack.Authentication.Identity;

/// <summary>
/// When the user logs in, the environment information of the current user can be obtained
/// </summary>
public interface IMultiEnvironmentUserContext : IUserContext
{
    string? Environment { get; }
}
