namespace Raccoon.Stack.Authentication.Identity.Entities;

public interface IIdentityUser
{
    string Id { get; set; }

    string? UserName { get; set; }

    string[] Roles { get; set; }
}
