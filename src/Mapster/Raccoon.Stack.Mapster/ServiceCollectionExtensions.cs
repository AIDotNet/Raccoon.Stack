namespace Raccoon.Stack.Mapster;

public static class ServiceCollectionExtensions
{
    public static void AddMapster(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IMapper, Mapper>();
    }
}