﻿using System.Reflection;

namespace Raccoon.Stack.Configuration;


public class RaccoonAppConfigureOptions
{
    public string AppId { get => GetValue(nameof(AppId)); set => Data[nameof(AppId)] = value; }

    public string Environment { get => GetValue(nameof(Environment)); set => Data[nameof(Environment)] = value; }

    public string Cluster { get => GetValue(nameof(Cluster)); set => Data[nameof(Cluster)] = value; }

    private Dictionary<string, string> Data { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    private List<VariableInfo> VariableInfos { get; set; } = new()
    {
        new VariableInfo(nameof(AppId), nameof(AppId),
            (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name!.Replace(".", "-")),
        new VariableInfo(nameof(Environment), "ASPNETCORE_ENVIRONMENT", "Production"),
        new VariableInfo(nameof(Cluster), nameof(Cluster), "Default")
    };

    public int Length => Data.Count;

    public string GetValue(string key) => GetValue(key, () => string.Empty);

    public string GetValue(string key, Func<string> defaultFunc)
    {
        if (Data.ContainsKey(key)) return Data[key];

        return defaultFunc.Invoke();
    }

    public bool TryAdd(string key, string value) => Data.TryAdd(key, value);

    public void Set(string key, string value) => Data[key] = value;

    public void TryRemove(string key)
    {
        if (Data.ContainsKey(key)) Remove(key);
    }

    public bool Remove(string key) => Data.Remove(key);

    public bool TrySetVariable(string key, string variable, string defaultValue)
    {
        if (VariableInfos.Any(v => v.Key.Equals(key, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        VariableInfos.Add(new VariableInfo(key, variable, defaultValue));
        return true;
    }

    public void SetVariable(string key, string variable, string defaultValue)
    {
        var variableInfo = VariableInfos.FirstOrDefault(v => v.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        if (variableInfo != null) VariableInfos.Remove(variableInfo);

        VariableInfos.Add(new VariableInfo(key, variable, defaultValue));
    }

    public VariableInfo? GetVariable(string key)
        => VariableInfos.FirstOrDefault(v => v.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<string> GetVariableKeys() => VariableInfos.Select(v => v.Key);
}
