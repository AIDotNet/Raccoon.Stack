using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Raccoon.Stack.Data.Options;

namespace Raccoon.Stack.Data.Extensions;

public static class SerializationBuilderExtensions
{
    public static void UseJson(this SerializationBuilder serializationBuilder, Action<JsonSerializerOptions>? configure = null)
    {
        JsonSerializerOptions? jsonSerializerOptions = null;
        if (configure != null)
        {
            jsonSerializerOptions = new JsonSerializerOptions();
            configure.Invoke(jsonSerializerOptions);
        }

        serializationBuilder.Services.TryAddJsonCore(jsonSerializerOptions);
        serializationBuilder.UseSerialization(
            serviceProvider => new DefaultJsonSerializer(JsonSerializerOptionsHelper.GetJsonSerializerOptions(serviceProvider, jsonSerializerOptions)),
            serviceProvider => new DefaultJsonDeserializer(JsonSerializerOptionsHelper.GetJsonSerializerOptions(serviceProvider, jsonSerializerOptions)));
    }

    private static void TryAddJsonCore(this IServiceCollection services, JsonSerializerOptions? jsonSerializerOptions)
    {
        services.TryAddSingleton<ISerializer>(serviceProvider
            => new DefaultJsonSerializer(JsonSerializerOptionsHelper.GetJsonSerializerOptions(serviceProvider, jsonSerializerOptions)));

        services.TryAddSingleton<IDeserializer>(serviceProvider
            => new DefaultJsonDeserializer(JsonSerializerOptionsHelper.GetJsonSerializerOptions(serviceProvider, jsonSerializerOptions)));
    }
    
    
    public static void UseSerialization(
        this SerializationBuilder serializationBuilder,
        Func<IServiceProvider, ISerializer> serializerFunc,
        Func<IServiceProvider, IDeserializer> deserializerFunc)
    {
        serializationBuilder.Services.Configure<SerializerFactoryOptions>(
            options => options.TryAdd(serializationBuilder.Name, serializerFunc));

        serializationBuilder.Services.Configure<DeserializerFactoryOptions>(
            options => options.TryAdd(serializationBuilder.Name, deserializerFunc));
    }
}