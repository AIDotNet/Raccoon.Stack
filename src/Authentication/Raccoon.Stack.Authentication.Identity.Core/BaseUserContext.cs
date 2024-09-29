using Raccoon.Stack.Authentication.Identity.Entities;

namespace Raccoon.Stack.Authentication.Identity.Core;

public class BaseUserContext : IUserContext
{
    private readonly IUserContext _userContext;

    public bool IsAuthenticated => _userContext.IsAuthenticated;
    public string? UserId => _userContext.UserId;
    public string? UserName => _userContext.UserName;

    public string GetUserId<TUserId>() =>
        _userContext.UserId ?? throw new InvalidOperationException("User is not authenticated");

    public IdentityUser? GetUser() => GetUser<IdentityUser>();

    public TIdentityUser? GetUser<TIdentityUser>() where TIdentityUser : IIdentityUser
        => _userContext.GetUser<TIdentityUser>();

}