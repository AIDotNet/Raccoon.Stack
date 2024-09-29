using Raccoon.Stack.Authentication.Identity.Entities;

namespace Raccoon.Stack.Authentication.Identity;

public interface IUserContext
{
    bool IsAuthenticated { get; }

    string? UserId { get; }

    string? UserName { get; }

    IdentityUser? GetUser();

    TIdentityUser? GetUser<TIdentityUser>() where TIdentityUser : IIdentityUser;
}
