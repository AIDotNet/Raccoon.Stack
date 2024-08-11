using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Raccoon.Stack.Data;

internal static class JsonSerializerOptionsHelper
{
    public static JsonSerializerOptions? GetJsonSerializerOptions(
        IServiceProvider serviceProvider,
        JsonSerializerOptions? jsonSerializerOptions)
    {
        return jsonSerializerOptions ??
               serviceProvider.GetService<IOptionsFactory<JsonSerializerOptions>>()
                   ?.Create(Microsoft.Extensions.Options.Options.DefaultName) ??
               RaccoonApp.GetJsonSerializerOptions();
    }
}