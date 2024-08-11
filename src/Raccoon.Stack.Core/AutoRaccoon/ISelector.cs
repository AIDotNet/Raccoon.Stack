using Microsoft.Extensions.DependencyInjection;

namespace Raccoon.Stack.Core.AutoRaccoon;

public interface ISelector
{
    void Populate(IServiceCollection services, RegistrationStrategy? options);
}