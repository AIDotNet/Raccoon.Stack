using Raccoon.Stack.Authentication.Identity.Entities;
using Raccoon.Stack.EntityFrameworkCore;

namespace Raccoon.Stack.Authentication.Identity.Core;

public class BaseUserContext : IUserContext
{
    private readonly IUserContext _userContext;
    protected ITypeConvertProvider TypeConvertProvider { get; }

    public bool IsAuthenticated => _userContext.IsAuthenticated;
    public string? UserId => _userContext.UserId;
    public string? UserName => _userContext.UserName;

    public BaseUserContext(IUserContext userContext, ITypeConvertProvider typeConvertProvider)
    {
        _userContext = userContext;
        TypeConvertProvider = typeConvertProvider;
    }

    public TUserId? GetUserId<TUserId>() => _userContext.GetUserId<TUserId>();

    public IdentityUser? GetUser() => GetUser<IdentityUser>();

    public TIdentityUser? GetUser<TIdentityUser>() where TIdentityUser : IIdentityUser
        => _userContext.GetUser<TIdentityUser>();

    public IEnumerable<TRoleId> GetUserRoles<TRoleId>() => _userContext.GetUserRoles<TRoleId>();
}
