namespace Raccoon.Stack.Authentication.Identity.Entities.Isolation;

public interface IMultiEnvironmentIdentityUser : IIdentityUser
{
    string? Environment { get; set; }
}
