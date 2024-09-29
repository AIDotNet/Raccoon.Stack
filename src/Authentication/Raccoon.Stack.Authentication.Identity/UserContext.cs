using Raccoon.Stack.Authentication.Identity.Entities;

namespace Raccoon.Stack.Authentication.Identity;

public abstract class UserContext : IUserContext
{
    private readonly AsyncLocal<Dictionary<Type, object?>?> _currentUser = new();

    public bool IsAuthenticated => GetUser() != null;

    public string? UserId => GetUser()?.Id;

    public string? UserName => GetUser()?.UserName;

    protected virtual Dictionary<Type, object?> CurrentUser => _currentUser.Value ??= new Dictionary<Type, object?>();


    protected abstract object? GetUser(Type userType);

    public IdentityUser? GetUser() => GetUser<IdentityUser>();

    public TIdentityUser? GetUser<TIdentityUser>() where TIdentityUser : IIdentityUser
    {
        var userModelType = typeof(TIdentityUser);
        if (!CurrentUser.TryGetValue(userModelType, out var user) || user == null)
        {
            user ??= GetUser(userModelType);
            CurrentUser.TryAdd(userModelType, user);
        }

        return user == null ? default : (TIdentityUser)user;
    }

    public IDisposable Change<TIdentityUser>(TIdentityUser identityUser) where TIdentityUser : IIdentityUser
    {
        ArgumentNullException.ThrowIfNull(identityUser);

        var userModelType = identityUser.GetType();
        var user = GetUser(userModelType);
        CurrentUser[userModelType] = identityUser;
        return new DisposeAction(() => CurrentUser[userModelType] = user);
    }
}