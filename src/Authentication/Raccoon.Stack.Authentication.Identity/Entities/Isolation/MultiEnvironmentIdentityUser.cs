namespace Raccoon.Stack.Authentication.Identity.Entities.Isolation;

public class MultiEnvironmentIdentityUser : IdentityUser
{
    public string? Environment { get; set; }
}
