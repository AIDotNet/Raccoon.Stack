using System.Collections;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Raccoon.Stack.Authentication.Identity.Core.Extensions;
using Raccoon.Stack.Utils.Caching;

namespace Raccoon.Stack.Authentication.Identity.Core;

public class DefaultUserContext : UserContext
{
    private readonly IOptionsMonitor<IdentityClaimOptions> _optionsMonitor;
    private static readonly MemoryCache<Type, CustomizeModelRelation> ModelRelationCache = new();

    private ClaimsPrincipal? ClaimsPrincipal { get; set; }

    public DefaultUserContext(
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IOptionsMonitor<IdentityClaimOptions> optionsMonitor)
        : base()
    {
        _optionsMonitor = optionsMonitor;
        _optionsMonitor.CurrentValue.Initialize();
        ClaimsPrincipal = currentPrincipalAccessor.GetCurrentPrincipal();
    }

    protected override object? GetUser(Type userType)
    {
        var userClaimType = _optionsMonitor.CurrentValue.GetClaimType(nameof(_optionsMonitor.CurrentValue.UserId))!;
        var userId = ClaimsPrincipal?.FindClaimValue(userClaimType);
        if (userId == null)
            return null;

        var modelRelation = ModelRelationCache.GetOrAdd(userType, (type) =>
        {
            var constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                                  .FirstOrDefault(c => c.GetParameters().Length == 0) ??
                              throw new InvalidOperationException($"[{type.Name}] has a parameterless constructor");
            return new CustomizeModelRelation(
                InstanceBuilder.CreateInstanceDelegate(constructor),
                InstanceBuilder.GetPropertyAndMethodInfoRelations(type));
        });
        var userModel = modelRelation.Func.Invoke(Array.Empty<object>());
        foreach (var property in userType.GetProperties())
        {
            var claimType = _optionsMonitor.CurrentValue.GetClaimType(property.Name);
            if (claimType == null)
                continue;

            string? claimValue = ClaimsPrincipal?.FindClaimValue(claimType);
            if (claimValue != null)
            {
                object? claimTypeValue = null;

                try
                {
                    claimTypeValue = JsonSerializer.Deserialize(claimValue, property.PropertyType);
                }
                catch
                {
                    claimTypeValue = this.ParseNonJson(property);
                }

                modelRelation.Setters[property]
                    .Invoke(userModel, new[] { claimTypeValue });
            }
        }

        return userModel;
    }

    private object? ParseNonJson(PropertyInfo property)
    {
        var claimValues = new List<string>();
        var claimType = _optionsMonitor.CurrentValue.GetClaimType(property.Name);
        if (claimType == null)
            return null;

        if (property.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
        {
            var claimsValues = ClaimsPrincipal?.Claims.Where(claim => claim.Type == claimType)
                .Select(claim => claim.Value).ToList();

            claimsValues?.ForEach(item =>
            {
                try
                {
                    var claimsValue = JsonSerializer.Deserialize<List<string>>(item);
                    if (claimsValue?.Any() == true)
                        claimValues.AddRange(claimsValue);
                }
                catch
                {
                    claimValues.Add(item);
                }
            });
        }

        return JsonSerializer.Deserialize(JsonSerializer.Serialize(claimValues), property.PropertyType);
    }
}