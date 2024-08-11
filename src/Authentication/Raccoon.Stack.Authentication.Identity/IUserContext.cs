using Raccoon.Stack.Authentication.Identity.Entities;

namespace Raccoon.Stack.Authentication.Identity;

public interface IUserContext
{
    bool IsAuthenticated { get; }

    string? UserId { get; }

    string? UserName { get; }

    TUserId? GetUserId<TUserId>();

    IdentityUser? GetUser();

    TIdentityUser? GetUser<TIdentityUser>() where TIdentityUser : IIdentityUser;

    IEnumerable<TRoleId> GetUserRoles<TRoleId>();
}
