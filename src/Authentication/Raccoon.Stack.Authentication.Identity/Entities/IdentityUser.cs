namespace Raccoon.Stack.Authentication.Identity.Entities;

public class IdentityUser : IIdentityUser
{
    public string Id { get; set; }

    public string? UserName { get; set; }

    public string[] Roles { get; set; } = Array.Empty<string>();
}
